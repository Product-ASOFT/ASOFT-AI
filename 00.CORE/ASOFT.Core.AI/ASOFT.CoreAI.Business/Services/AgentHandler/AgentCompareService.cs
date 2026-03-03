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
            IEnumerable<RedisearchResultItem> trainingData)
        {
            request.Question = "Hãy đối chiếu dữ liệu do người dùng cung cấp theo các tiêu chí dưới đây";
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
                    trainingData,
                    new List<BEMF2000ViewModel> { detail },
                    request.BEMF2001ViewModels ?? new List<BEMF2001ViewModel>()
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
        public async Task<string> FormatOCRText(string ocrText, ST2131 sT2131, string promptContent, string promptContentSystem)
        {
            var resultJson = await _agentPromptService.SendPromptWithSumaryResultAsync(promptContentSystem, promptContent, ocrText);
            if (resultJson == null)
                return string.Empty;
            string resultJsonFormat = StripOutsideJson(resultJson);
            try
            {
                await ConvertAiJsonToST2137_2138(resultJsonFormat, sT2131);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi convert JSON từ AI sang ST2137/2138: {Json}", resultJsonFormat);
                return string.Empty;
            }
            return resultJsonFormat;
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

        public async Task ConvertAiJsonToST2137_2138(string aiJson, ST2131 sT2131)
        {
            var settings = new JsonSerializerSettings
            {
                Culture = CultureInfo.GetCultureInfo("vi-VN"),
                DateParseHandling = DateParseHandling.DateTime
            };
            var result = JsonConvert.DeserializeObject<AiNormalizeResult>(aiJson, settings);
            if (result == null)
                return;

            var masters = new List<ST2137>();
            var details = new List<ST2138>();

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
                }
            }
            if (masters.Count == 0 || details.Count == 0)
                return;
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

        }
    }
}