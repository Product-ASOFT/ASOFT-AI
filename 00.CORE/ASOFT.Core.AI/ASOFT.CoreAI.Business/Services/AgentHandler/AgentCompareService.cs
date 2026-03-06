using ASOFT.A00.Entities;
using ASOFT.Core.Common.InjectionChecker;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using static ASOFT.CoreAI.Common.AIConstants;
using static ASOFT.CoreAI.Common.EnumConstants;
using JsonException = Newtonsoft.Json.JsonException;

namespace ASOFT.CoreAI.Business
{
    public class AgentCompareService
    {
        private readonly AgentPromptService _agentPromptService;
        private readonly SettingsManagerService _settings;
        private readonly IST2137Queries _ST2137Queries;
        private readonly IST2138Queries _ST2138Queries;
        private readonly ILogger _logger;

        public AgentCompareService(AgentPromptService agentPromptService,
            SettingsManagerService settings,
            IST2137Queries ST2137Queries,
            IST2138Queries ST2138Queries,
            ILoggerFactory logger)
        {
            _agentPromptService = agentPromptService;
            _agentPromptService = agentPromptService;
            _settings = settings;
            _ST2137Queries = ST2137Queries;
            _ST2138Queries = ST2138Queries;
            _logger = Checker.NotNull(logger, nameof(logger)).CreateLogger(GetType());
        }

        public async Task<string> CompareAsync(
            ReadFileRequest request,
            string promptSystem,
            string promptTemplate,
            string? ocrTextMerged,
            List<ResultReadFileModel>? ocrResults,
            List<AISectionCompare> aiSectionCompares,
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
                    promptTemplate,
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
                promptTemplate,
                ocrResults ?? new List<ResultReadFileModel>(),
                Enumerable.Empty<ChatHistoryResponseModel>(),
                trainingData,
                new List<BEMF2000ViewModel> { detail },
                request.BEMF2001ViewModels ?? new List<BEMF2001ViewModel>()
            ).ConfigureAwait(false);
        }

        public async Task<CriteriaSummaryResult?> SummaryResultJson(string result)
        {
            // 1. Cắt block summary nếu có
            //result = ExtractSummaryBlock(result);

            // 2. Lấy prompt
            var prompt = await _agentPromptService.GetPromptByCode(AgentKeys.BEM_AGENT_BEMF2000_SUMMARY);

            if (string.IsNullOrWhiteSpace(prompt.PromptContent) || string.IsNullOrWhiteSpace(prompt.Description))
                return null;

            // 3. Gửi prompt cho LLM
            var resultJson = await _agentPromptService.SendPromptWithSumaryResultAsync(prompt.Description, prompt.PromptContent, result);


            if (string.IsNullOrWhiteSpace(resultJson))
                return null;

            // 4. Strip text ngoài JSON (```json ... ```, text giải thích...)
            var resultJsonFormat = StripOutsideJson(resultJson);

            if (string.IsNullOrWhiteSpace(resultJsonFormat))
                return null;

            resultJsonFormat = resultJsonFormat.Trim();

            // 5. CHECK CÓ PHẢI JSON KHÔNG
            if (!IsValidJson(resultJsonFormat))
            {
                // Log để bắt bệnh LLM
                _logger.LogError("LLM trả dữ liệu không phải JSON: {Raw}", resultJsonFormat);
                return null;
            }

            // 6. Deserialize an toàn
            try
            {
                var objectResult =
                    System.Text.Json.JsonSerializer.Deserialize<CriteriaSummaryResult>(
                        resultJsonFormat,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                return objectResult;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Lỗi parse JSON: {Json}", resultJsonFormat);
                return null;
            }
        }

        public ST2136? ParseCriteriaResult(ST2131 entity, ReadFileRequest request, string json, int criteriaID)
        {
            try
            {
                json = StripOutsideJson(json);
                var settings = new JsonSerializerSettings
                {
                    Culture = CultureInfo.GetCultureInfo("vi-VN"),
                    DateParseHandling = DateParseHandling.DateTime
                };
                var result = JsonConvert.DeserializeObject<CriteriaSummaryResult>(json, settings);
                if (result != null && result.Criteria != null)
                {
                    var criteria = result.Criteria;
                    var ST2136 = new ST2136
                    {
                        APK = Guid.NewGuid(),
                        APKMaster = entity.APK,
                        CriteriaName = criteria.CriteriaName,
                        CriteriaStatus = criteria.CriteriaStatus,
                        CreateDate = DateTime.Now,
                        CreateUserID = entity.CreateUserID,
                        BusinessParent = request.BEMF2000ViewModel!.VoucherNo,
                        Description = criteria.Description,
                        CriteriaID = criteriaID
                    };
                    if (criteria.CriteriaStatus == StatusResultCompare.BLANK.ToString())
                    {
                        ST2136.CriteriaStatus = StatusResultCompare.NG.ToString();
                    }
                    return ST2136;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi convert JSON từ AI sang ST2136: {Json}", json);
                return null;
            }
        }
        private static string ExtractSummaryBlock(string aiText)
        {
            var match = Regex.Match(aiText, @"tổng\s*hợp", RegexOptions.IgnoreCase);
            var startIndex = match.Success ? match.Index : -1;
            if (startIndex < 0)
                return string.Empty;

            var nextIndex = aiText.IndexOf("## 4.", startIndex, StringComparison.OrdinalIgnoreCase);
            if (nextIndex > startIndex)
                return aiText.Substring(startIndex, nextIndex - startIndex);

            return aiText.Substring(startIndex);
        }
        public async Task<List<AISectionCompare>?> FormatOCRText(string ocrText, ST2131 sT2131, string promptContent, string promptContentSystem)
        {
            var resultJson = await _agentPromptService.SendPromptWithSumaryResultAsync(promptContentSystem, promptContent, ocrText);
            if (resultJson == null)
                return null;
            string resultJsonFormat = StripOutsideJson(resultJson);
            try
            {
                var aiNormalizeResult = ConvertAiJsonToST2137_2138(resultJsonFormat);
                if (aiNormalizeResult == null)
                {
                    return null;
                }
                return await SaveInfomationFileAsync(aiNormalizeResult, sT2131);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi convert JSON từ AI sang ST2137/2138: {Json}", resultJsonFormat);
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

        public AiNormalizeResult? ConvertAiJsonToST2137_2138(string aiJson)
        {
            var settings = new JsonSerializerSettings
            {
                Culture = CultureInfo.GetCultureInfo("vi-VN"),
                DateParseHandling = DateParseHandling.DateTime
            };
            var result = JsonConvert.DeserializeObject<AiNormalizeResult>(aiJson, settings);
            if (result == null)
                return null;
            return result;
        }
        /// <summary>
        /// Lưu thông tin đã được chuẩn hóa từ AI vào database theo cấu trúc ST2137 (Master) và ST2138 (Detail)
        /// </summary>
        /// <param name="result"></param>
        /// <param name="sT2131"></param>
        /// <returns></returns>
        private async Task<List<AISectionCompare>> SaveInfomationFileAsync(AiNormalizeResult result, ST2131 sT2131)
        {
            var masters = new List<ST2137>();
            var details = new List<ST2138>();
            var lstAISectionCompare = new List<AISectionCompare>();
            foreach (var section in result.Sections)
            {
                var masterApk = Guid.NewGuid();
                var master = new ST2137
                {
                    APK = masterApk,
                    APKMaster_ST2131 = sT2131.APK,
                    DivisionID = sT2131.DivisionID,
                    SectionType = section.Master.SectionType,
                    SectionOrder = section.Master.SectionOrder,
                    SectionTitle = section.Master.SectionTitle,
                    TotalAmount = section.Master.TotalAmount,
                    TotalCurrency = section.Master.TotalCurrency,
                    Signature = section.Master.Signature,
                    CreateDate = DateTime.Now,
                    CreateUserID = sT2131.CreateUserID,
                };
                masters.Add(master);
                int orderNo = 1;
                foreach (var d in section.Details)
                {
                    var detail = new ST2138
                    {
                        APK = Guid.NewGuid(),
                        APKMaster_ST2131 = sT2131.APK,
                        APKMaster_ST2137 = masterApk,
                        OrderNo = d.OrderNo,
                        VoucherNo = d.VoucherNo,
                        VoucherName = d.VoucherName,
                        Amount = d.Amount ?? 0,
                        Currency = d.Currency,
                        SupplierName = d.SupplierName,
                        VoucherDate = d.VoucherDate,
                        FileName = d.FileName,
                        PaymentTerm = d.PaymentTerm,
                        DeliveryTerm = d.DeliveryTerm,
                        ClearanceStatus = d.ClearanceStatus,
                        ClearanceDate = d.ClearanceDate,
                        AcceptanceDate = d.AcceptanceDate,
                        HandoverDate = d.HandoverDate,
                        PackingListDate = d.PackingListDate,
                        RingiNo = d.RingiNo,
                        ContractNo = d.ContractNo,
                        PackingListNo = d.PackingListNo,
                        BillNo = d.BillNo,
                        BillDate = d.BillDate,
                        DeclarationNo = d.DeclarationNo,
                        GoodsName = d.GoodsName,
                        Quantity = d.Quantity,
                        ApprovalLast = d.ApprovalLast,
                        CreateDate = DateTime.Now,
                        CreateUserID = sT2131.CreateUserID,
                        Description = d.Description,
                    };
                    details.Add(detail);
                    var AISectionCompare = new AISectionCompare
                    {
                        NoOrder = orderNo++,
                        SectionType = master.SectionType,
                        SupplierName = d.SupplierName,
                        VoucherNo = d.VoucherNo,
                        Amount = d.Amount,
                        Currency = d.Currency,
                        CompleteCheckDate = d.CompleteCheckDate,
                        DeliveryTerm = d.DeliveryTerm,
                        Signature = section.Master.Signature,
                        PaymentTerm = d.PaymentTerm,
                        VoucherDate = d.VoucherDate,
                        FileName = d.FileName,
                        AmountCustomSheet = master.SectionType == "CUSTOMSHEET" ? d.Amount : 0,
                    };
                    lstAISectionCompare.Add(AISectionCompare);
                }
            }
            if (masters.Count == 0 || details.Count == 0)
                return lstAISectionCompare;
            try
            {
                // Lưu dữ liệu ở lần đối chiếu mới
                await _ST2137Queries.SaveData(masters);
                await _ST2138Queries.SaveData(details);
            }
            catch (Exception)
            {
                throw;
            }
            return lstAISectionCompare;
        }
    }
}