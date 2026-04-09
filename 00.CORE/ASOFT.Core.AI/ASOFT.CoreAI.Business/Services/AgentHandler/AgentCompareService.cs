using ASOFT.A00.Entities;
using ASOFT.Core.Common.InjectionChecker;
using ASOFT.CoreAI.Common;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static ASOFT.CoreAI.Common.AIConstants;
using static ASOFT.CoreAI.Common.EnumConstants;
using JsonException = Newtonsoft.Json.JsonException;

namespace ASOFT.CoreAI.Business
{
    public class AgentCompareService
    {
        private readonly AgentPromptService _agentPromptService;
        private readonly SettingsManagerService _settings;
        private readonly IBEMT2005Queries _BEMT2005Queries;
        private readonly IBEMT2006Queries _BEMT2006Queries;
        private readonly DynamicConfigService _dynamicConfigService;
        private readonly ILogger _logger;

        public AgentCompareService(AgentPromptService agentPromptService,
            SettingsManagerService settings,
            IBEMT2005Queries BEMT2005Queries,
            IBEMT2006Queries BEMT2006Queries,
            DynamicConfigService dynamicConfigService,
            ILoggerFactory logger)
        {
            _agentPromptService = agentPromptService;
            _agentPromptService = agentPromptService;
            _settings = settings;
            _BEMT2005Queries = BEMT2005Queries;
            _BEMT2006Queries = BEMT2006Queries;
            _dynamicConfigService = dynamicConfigService;
            _logger = Checker.NotNull(logger, nameof(logger)).CreateLogger(GetType());
        }

        public async Task<string> CompareAsync(
            ReadFileRequest request,
            string promptSystem,
            string promptUser,
            string? ocrTextMerged,
            List<ResultReadFileModel>? ocrResults,
           List<Dictionary<string, object?>> aiSectionCompares,
            IEnumerable<RedisearchResultItem>? trainingData = null)
        {
            request.Question = "Hãy dùng thông tin dưới đây để so sánh tiêu chí";
            var useLocal = await _settings.GetIsUseServiceReadOCRAsync();
            var detail = request!.BEMF2000ViewModel ?? new BEMF2000ViewModel();

            if (useLocal)
            {
                return await _agentPromptService.SendPromptWithLocalsAsync(
                    request,
                    promptSystem,
                    promptUser,
                    ocrTextMerged ?? string.Empty,
                    Enumerable.Empty<ChatHistoryResponseModel>(),
                    new List<BEMF2000ViewModel> { detail },
                    request.BEMF2001ViewModels ?? new List<BEMF2001ViewModel>(),
                    aiSectionCompares
                ).ConfigureAwait(false);
            }

            return await _agentPromptService.SendPromptWithReadFile(
                request,
                promptSystem,
                promptUser,
                ocrResults ?? new List<ResultReadFileModel>(),
                Enumerable.Empty<ChatHistoryResponseModel>(),
                trainingData,
                new List<BEMF2000ViewModel> { detail },
                request.BEMF2001ViewModels ?? new List<BEMF2001ViewModel>()
            ).ConfigureAwait(false);
        }

        public BEMT2004? ParseCriteriaResult(BEMT2003 entity, ReadFileRequest request, string json, int criteriaID, string criteriaInfoName)
        {
            try
            {
                var parsedResult = BuildCriteriaResult(json, criteriaInfoName);

                return new BEMT2004
                {
                    APK = Guid.NewGuid(),
                    APKMaster = entity.APK,
                    CriteriaName = parsedResult.CriteriaName,
                    CriteriaStatus = parsedResult.CriteriaStatus,
                    CreateDate = DateTime.Now,
                    CreateUserID = entity.CreateUserID,
                    BusinessParent = request.BEMF2000ViewModel!.VoucherNo,
                    Description = parsedResult.Description,
                    CriteriaID = criteriaID,
                    FileName = parsedResult.FileName,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi convert JSON từ AI sang BEMT2004: {Json}", json);
                return null;
            }
        }

        private ParsedCriteriaResult BuildCriteriaResult(string json, string defaultCriteriaName)
        {
            if (json == AIConstants.OUTMEMORY)
            {
                return ParsedCriteriaResult.Fail(defaultCriteriaName, StatusResultCompare.NG.ToString(), "Không đủ bộ nhớ để xử lý đối chiếu tiêu chí này");
            }
            var cleanedJson = StripOutsideJson(json);

            try
            {
                var settings = new JsonSerializerSettings
                {
                    Culture = CultureInfo.GetCultureInfo("vi-VN"),
                    DateParseHandling = DateParseHandling.DateTime
                };

                var result = JsonConvert.DeserializeObject<CriteriaSummaryResult>(cleanedJson, settings);
                var criteria = result?.Criteria;

                if (criteria == null)
                {
                    return ParsedCriteriaResult.Fail(defaultCriteriaName, StatusResultCompare.NG.ToString(), "Không có thông tin. Vui lòng thử lại");
                }

                var finalStatus = criteria.CriteriaStatus == StatusResultCompare.BLANK.ToString()
                    ? StatusResultCompare.NG.ToString()
                    : criteria.CriteriaStatus;

                return new ParsedCriteriaResult
                {
                    CriteriaName = string.IsNullOrWhiteSpace(criteria.CriteriaName) ? defaultCriteriaName : criteria.CriteriaName,
                    CriteriaStatus = finalStatus,
                    Description = criteria.Description,
                    FileName = criteria.FileName,
                };
            }
            catch (Exception)
            {
                return ParsedCriteriaResult.Fail(defaultCriteriaName, StatusResultCompare.NG.ToString(), "Không thể xử lý kết quả đối chiếu. Vui lòng thử lại.");
            }
        }

        public async Task<List<Dictionary<string, object?>>?> FormatOCRText(string ocrText, BEMT2003 BEMT2003, string promptContent, string promptContentSystem, string fileName)
        {
            //var resultJson = await _agentPromptService.SendPromptWithSumaryResultAsync(promptContentSystem, promptContent, ocrText);
            string resultJson = @"{
  ""sections"": [
    {
      ""master"": {
        ""SectionOrder"": 1,
        ""SectionType"": ""INVOICE"",
        ""SectionTitle"": ""INVOICE"",
        ""TotalAmount"": 71851000,
        ""TotalCurrency"": ""USD"",
        ""Signature"": ""BLANK""
      },
      ""details"": [
        {
          ""OrderNo"": ""1"",
          ""VoucherNo"": ""28484 TT"",
          ""VoucherDate"": ""2025-12-26"",
          ""Amount"": 71851000,
          ""Currency"": ""USD"",
          ""SupplierName"": ""MEIKO ELECTRONICS VIETNAM CO.,LTD."",
          ""DeliveryTerm"": ""FOB YOKOHAMA""
        }
      ]
    },
    {
      ""master"": {
        ""SectionOrder"": 2,
        ""SectionType"": ""PACKINGLIST"",
        ""SectionTitle"": ""PACKING LIST"",
        ""TotalAmount"": 0,
        ""TotalCurrency"": null,
        ""Signature"": ""BLANK""
      },
      ""details"": [
        {
          ""OrderNo"": ""1"",
          ""PackingListNo"": ""28484TT"",
          ""PackingListDate"": ""2025-12-02"",
          ""GoodsName"": ""Clean roller type cleaning machine"",
          ""Quantity"": 1,
          ""SupplierName"": ""MEIKO ELECTRONICS VIETNAM CO.,LTD.""
        },
        {
          ""OrderNo"": ""2"",
          ""PackingListNo"": ""28484TT"",
          ""PackingListDate"": ""2025-12-02"",
          ""GoodsName"": ""MC-2000 Robo Sticky"",
          ""Quantity"": 1,
          ""SupplierName"": ""MEIKO ELECTRONICS VIETNAM CO.,LTD.""
        },
        {
          ""OrderNo"": ""3"",
          ""PackingListNo"": ""28484TT"",
          ""PackingListDate"": ""2025-12-02"",
          ""GoodsName"": ""Cleaning tape for MC-2000"",
          ""Quantity"": 1,
          ""SupplierName"": ""MEIKO ELECTRONICS VIETNAM CO.,LTD.""
        }
      ]
    }
  ]
}";
            if (resultJson == null)
                return null;
            string resultJsonFormat = StripOutsideJson(resultJson);
            try
            {
                var dataProcess = await ProcessInfomationFileAsync(resultJsonFormat, BEMT2003, fileName);
                return dataProcess;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi convert JSON từ AI sang BEMT2005/BEMT2006: {Json}", resultJsonFormat);
                return null;
            }
        }
        private bool IsValidJson(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            input = input.Trim();

            // Trường hợp JSON bị bọc trong dấu "
            if ((input.StartsWith("\"") && input.EndsWith("\"")) ||
                (input.StartsWith("'") && input.EndsWith("'")))
            {
                try
                {
                    input = System.Text.Json.JsonSerializer.Deserialize<string>(input);
                }
                catch
                {
                    return false;
                }
            }
            if (string.IsNullOrEmpty(input))
                return false;
            try
            {
                using var doc = JsonDocument.Parse(input,
                    new JsonDocumentOptions
                    {
                        AllowTrailingCommas = true,
                        CommentHandling = JsonCommentHandling.Skip
                    });

                return true;
            }
            catch
            {
                return false;
            }
        }

        public string StripOutsideJson(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            input = input.Trim();

            if (IsValidJson(input))
                return input;

            int start = input.IndexOf('{');
            if (start == -1)
                return string.Empty;

            int bracket = 0;
            int end = -1;

            for (int i = start; i < input.Length; i++)
            {
                if (input[i] == '{') bracket++;
                else if (input[i] == '}') bracket--;

                if (bracket == 0)
                {
                    end = i;
                    break;
                }
            }

            if (end == -1)
                return string.Empty;

            var json = input.Substring(start, end - start + 1);

            return IsValidJson(json) ? json : string.Empty;
        }

        public AiNormalizeResult? ConvertAiJsonToBEMT2005_BEMT2006(string aiJson)
        {
            if (string.IsNullOrWhiteSpace(aiJson))
                return null;

            var settings = new JsonSerializerSettings
            {
                Culture = CultureInfo.GetCultureInfo("vi-VN"),
                DateParseHandling = DateParseHandling.DateTime,
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            return JsonConvert.DeserializeObject<AiNormalizeResult>(aiJson, settings);
        }
        public async Task<List<Dictionary<string, object?>>> ProcessInfomationFileAsync(string aiJson, BEMT2003 bemt2003, string fileName)
        {

            // 1. Parse JSON
            var listDict = new List<Dictionary<string, object?>>();
            var aiResult = ConvertAiJsonToBEMT2005_BEMT2006(aiJson);
            if (aiResult == null || aiResult.Sections.Count == 0)
                return listDict;

            // 2. Load config
            var configs = await _dynamicConfigService.ConvertDynamicFieldConfig();
            var configMap = configs.ToDictionary(x => x.ParameterName!, StringComparer.OrdinalIgnoreCase);
            var listMaster = new List<BEMT2005>();
            var listDetail = new List<BEMT2006>();

            foreach (var section in aiResult.Sections)
            {
                // 3. Build master
                var master = _dynamicConfigService.BuildMaster(section, configMap, bemt2003.APK, bemt2003.CreateUserID);
                listMaster.Add(master);
                // save master

                // 4. Build detail
                var details = _dynamicConfigService.BuildDetails(section, master, configMap, master.APK, bemt2003.APK, bemt2003.CreateUserID, fileName);
                if (details == null || !details.Entities.Any())
                    continue;

                listDetail.AddRange(details.Entities);
                listDict.AddRange(details.Rows);
            }
            // save detail
            if (!listMaster.Any() || !listDetail.Any())
                return listDict;
            await _BEMT2005Queries.SaveData(listMaster);
            await _BEMT2006Queries.SaveData(listDetail);
            return listDict;
        }
    }
}