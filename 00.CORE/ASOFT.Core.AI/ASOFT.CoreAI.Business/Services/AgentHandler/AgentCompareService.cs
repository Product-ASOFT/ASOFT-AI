using ASOFT.CoreAI.Business;
using ASOFT.CoreAI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Business
{
    public class AgentCompareService
    {
        private readonly AgentPromptService _agentPromptService;
        private readonly SettingsManagerService _settings;

        public AgentCompareService(AgentPromptService agentPromptService, SettingsManagerService settings)
        {
            _agentPromptService = agentPromptService;
            _settings = settings;
        }
        public async Task<string> CompareAsync(
            ReadFileRequest request,
            string prompt,
            string? ocrTextMerged,
            List<ResultReadFileModel>? ocrResults,
            IEnumerable<RedisearchResultItem> trainingData)
        {
            request.Question = "Hãy đối chiếu dữ liệu đọc được từ OCR với dữ liệu ở người dùng cung cấp (datas) cho tôi";
            var useLocal = _settings.GetIsUseServiceReadOCR();
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
    }
}
