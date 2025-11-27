using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using System.Text.Json;
using System.Text.RegularExpressions;
using static ASOFT.CoreAI.Common.AIConstants;

namespace ASOFT.CoreAI.Business
{
    public class AgentCompareService
    {
        private readonly AgentPromptService _agentPromptService;
        private readonly SettingsManagerService _settings;
        private readonly IST2136Queries _ST2136Queries;

        public AgentCompareService(AgentPromptService agentPromptService, SettingsManagerService settings, IST2136Queries ST2136Queries)
        {
            _agentPromptService = agentPromptService;
            _settings = settings;
            _ST2136Queries = ST2136Queries;
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

        public async Task<CriteriaSummaryResult> SummaryResultJson(string result)
        {
            result = ExtractSummaryBlock(result);
            var promptContent = await _agentPromptService.GetPromptTemplate(AgentKeys.BEM_AGENT_BEMF2000_SUMMARY);
            if (promptContent == null)
                return null;
            var resultJson = await _agentPromptService.SendPromptWithSumaryResultAsync(promptContent, result).ConfigureAwait(false);
            if (resultJson == null)
                return null;
            var resultJsonFormat = StripOutsideJson(resultJson);
            var objectResult = JsonSerializer.Deserialize<CriteriaSummaryResult>(resultJsonFormat,
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
    }
}