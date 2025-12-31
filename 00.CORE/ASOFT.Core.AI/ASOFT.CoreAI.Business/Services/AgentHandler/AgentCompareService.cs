using ASOFT.A00.Entities;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using static ASOFT.CoreAI.Common.AIConstants;

namespace ASOFT.CoreAI.Business
{
    public class AgentCompareService
    {
        private readonly AgentPromptService _agentPromptService;
        private readonly SettingsManagerService _settings;
        private readonly IST2137Queries _ST2137Queries;
        private readonly IST2138Queries _ST2138Queries;

        public AgentCompareService(AgentPromptService agentPromptService,
            SettingsManagerService settings,
            IST2137Queries ST2137Queries,
            IST2138Queries ST2138Queries)
        {
            _agentPromptService = agentPromptService;
            _agentPromptService = agentPromptService;
            _settings = settings;
            _ST2137Queries = ST2137Queries;
            _ST2138Queries = ST2138Queries;
        }

        public async Task<string> CompareAsync(
            ReadFileRequest request,
            string prompt,
            string? ocrTextMerged,
            List<ResultReadFileModel>? ocrResults,
            IEnumerable<RedisearchResultItem> trainingData)
        {
            request.Question = "Hãy đối chiếu dữ liệu đọc được từ OCR với dữ liệu ở người dùng cung cấp (datas) cho tôi";
            var useLocal = await _settings.GetIsUseServiceReadOCRAsync();
            var detail = request!.BEMF2000ViewModel ?? new BEMF2000ViewModel();

            if (useLocal)
            {
                return await _agentPromptService.SendPromptWithLocalsAsync(
                    request,
                    prompt,
                    ocrTextMerged ?? string.Empty,
                    Enumerable.Empty<ChatHistoryResponseModel>(),
                    trainingData,
                    new List<BEMF2000ViewModel> { detail },
                    request.BEMF2001ViewModels ?? new List<BEMF2001ViewModel>()
                ).ConfigureAwait(false);
            }

            return await _agentPromptService.SendPromptWithReadFile(
                request,
                prompt,
                ocrResults ?? new List<ResultReadFileModel>(),
                Enumerable.Empty<ChatHistoryResponseModel>(),
                trainingData,
                new List<BEMF2000ViewModel> { detail },
                request.BEMF2001ViewModels ?? new List<BEMF2001ViewModel>()
            ).ConfigureAwait(false);
        }

        public async Task<CriteriaSummaryResult?> SummaryResultJson(string result)
        {
            result = ExtractSummaryBlock(result);
            var promptContent = await _agentPromptService.GetPromptTemplate(AgentKeys.BEM_AGENT_BEMF2000_SUMMARY);
            if (promptContent == null)
                return null;
            var resultJson = await _agentPromptService.SendPromptWithSumaryResultAsync(promptContent, result).ConfigureAwait(false);
            if (resultJson == null)
                return null;
            var resultJsonFormat = StripOutsideJson(resultJson);
            //var resultJsonFormat = StripOutsideJson(result);
            var objectResult = System.Text.Json.JsonSerializer.Deserialize<CriteriaSummaryResult>(resultJsonFormat,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return objectResult!;
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
        public static string StripOutsideJson(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            int start = input.IndexOf('{');
            if (start == -1)
                return string.Empty;

            int bracket = 0;
            int end = -1;

            for (int i = start; i < input.Length; i++)
            {
                if (input[i] == '{') bracket++;
                if (input[i] == '}') bracket--;

                if (bracket == 0)
                {
                    end = i;
                    break;
                }
            }

            if (end == -1)
                return string.Empty;

            return input.Substring(start, end - start + 1);
        }
        public async Task<string> FormatOCRText(string ocrText, ST2131 sT2131, string promptContent)
        {
            var resultJson = await _agentPromptService.SendPromptWithSumaryResultAsync(promptContent, ocrText).ConfigureAwait(false);
            if (resultJson == null)
                return string.Empty;
            resultJson = StripOutsideJson(resultJson);
            await ConvertAiJsonToST2137_2138(resultJson, sT2131);
            return resultJson;
        }
        public async Task ConvertAiJsonToST2137_2138(string aiJson, ST2131 sT2131)
        {
            var result = JsonConvert.DeserializeObject<AiNormalizeResult>(aiJson);
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
                        DeclarationNo = d.DeclarationNo,
                        GoodsName = d.GoodsName,
                        Quantity = d.Quantity,
                        ExtraJson = NormalizeExtraJson(d.ExtraJson),
                        CreateDate = DateTime.Now,
                        CreateUserID = sT2131.CreateUserID
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
        private string? NormalizeExtraJson(string? extraJson)
        {
            if (string.IsNullOrWhiteSpace(extraJson))
                return null;

            try
            {
                var token = JToken.Parse(extraJson);
                return token.ToString(Formatting.None);
            }
            catch
            {
                return extraJson;
            }
        }
    }
}