using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure.Interface;
using ASOFT.CoreAI.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ASOFT.CoreAI.Common.AIConstants;
using static ASOFT.CoreAI.Common.EnumConstants;

namespace ASOFT.CoreAI.Business
{
    public sealed class ReadFileBackgroundWorkflow : IReadFileBackgroundWorkflow
    {
        private readonly IST2131Queries _ST2131;
        private readonly IST2130Queries _ST2130;
        private readonly ITrainingDataService _trainingService;
        private readonly IOCRService _ocrService;
        private readonly AgentCompareService _compareService;
        private readonly ILogger<ReadFileBackgroundWorkflow> _logger;

        public ReadFileBackgroundWorkflow(
            IST2131Queries ST2131,
            IST2130Queries ST2130,
            IOCRService ocrService,
            ITrainingDataService trainingService,
            AgentCompareService compareService,
            ILogger<ReadFileBackgroundWorkflow> logger)
        {
            _ST2131 = ST2131;
            _ST2130 = ST2130;
            _ocrService = ocrService;
            _trainingService = trainingService;
            _compareService = compareService;
            _logger = logger;
        }

        public async Task RunAsync(Guid ST2131APK, ReadFileRequest request, string promptContent, CancellationToken ct = default)
        {
            var entity = await _ST2131.GetFileResult(ST2131APK);
            if (entity == null) return;

            try
            {
                if (request == null) throw new Exception("Không tìm thấy request.");

                // OCR
                var (ocrText, ocrResults) = await _ocrService.ReadAsync(request.AttachFiles!, request.BEMF2000ViewModel.APK);
                if (string.IsNullOrWhiteSpace(ocrText))
                    throw new Exception("Không có thông tin đọc được từ tệp đính kèm");

                // Training
                var trainingData = await _trainingService.GetTrainingDataAsync(request, AgentKeys.BEM_AGENT_BEMF2000);

                // Compare
                var aiResult = await _compareService.CompareAsync(request, promptContent, ocrText, ocrResults, trainingData);

                // Update kết quả
                entity.TextContentOCR = ocrText;
                entity.AttachID = request.AttachFiles!.Select(x => x.AttachID).FirstOrDefault();
                entity.TextContentAI = !string.IsNullOrWhiteSpace(aiResult) ? aiResult : "Không có kết quả đối chiếu";
                entity.StatusProcess = StatusProcessCompareOCR.COMPLETED.ToString();

                var match = ExtractMatchInfo.Extract(aiResult);
                if (!string.IsNullOrEmpty(match.MatchRate)) entity.Percentage = match.MatchRate;
                if (!string.IsNullOrEmpty(match.Conclusion)) entity.Status = match.Conclusion;

                await _ST2131.UpdateFileResult(entity);
            }
            catch (OperationCanceledException)
            {
                entity.StatusProcess = StatusProcessCompareOCR.FAILED.ToString();
                await _ST2131.UpdateFileResult(entity);
            }
            catch (Exception ex)
            {
                entity.StatusProcess = StatusProcessCompareOCR.FAILED.ToString();
                await _ST2131.UpdateFileResult(entity);
                _logger.LogError(ex, "ReadFile job failed for {APK}", ST2131APK);
            }
        }
        private string GetAgentKeyByTypeCompare(string typeCompare)
        {
            return typeCompare switch
            {
                "WareHouse" => AgentTypeKeys.BEM_AGENT_BEMF2000_WAREHOUSE,
                "Machine" => AgentTypeKeys.BEM_AGENT_BEMF2000_MACHINE,
                "Service" => AgentTypeKeys.BEM_AGENT_BEMF2000_SERVICE,
                "Build" => AgentTypeKeys.BEM_AGENT_BEMF2000_BUILD,
                "Other" => AgentTypeKeys.BEM_AGENT_BEMF2000_OTHER,
                _ => throw new NotImplementedException(),
            };
        }
    }

}
