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
        private readonly IST2136Queries _ST2136;

        private readonly ITrainingDataService _trainingService;
        private readonly IOCRService _ocrService;
        private readonly AgentCompareService _compareService;
        private readonly ILogger<ReadFileBackgroundWorkflow> _logger;
        private readonly AgentCompareService _agentCompareService;
        public ReadFileBackgroundWorkflow(
            IST2131Queries ST2131,
            IST2130Queries ST2130,
            IST2136Queries ST2136,
            IOCRService ocrService,
            ITrainingDataService trainingService,
            AgentCompareService compareService,
            ILogger<ReadFileBackgroundWorkflow> logger,
            AgentCompareService agentCompareService)
        {
            _ST2131 = ST2131;
            _ST2130 = ST2130;
            _ST2136 = ST2136;
            _ocrService = ocrService;
            _trainingService = trainingService;
            _compareService = compareService;
            _logger = logger;
            _agentCompareService = agentCompareService;
        }

        public async Task RunAsync(Guid ST2131APK, ReadFileRequest request, string promptContent, CancellationToken ct = default)
        {
            var entity = await _ST2131.GetFileResult(ST2131APK);
            if (entity == null) return;

            try
            {
                if (request == null) throw new Exception("Không tìm thấy request.");

                // OCR
                var (ocrText, ocrResults) = await _ocrService.ReadAsync(request.AttachFiles!, request.BEMF2000ViewModel!.APK);
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

                //await _ST2131.UpdateFileResult(entity);
                var voucherNo = request.BEMF2000ViewModel.VoucherNo!;
                var existingDetails = await _ST2136.GetResultDetail(voucherNo);

                // Xóa dữ liệu cũ nếu có
                if (existingDetails?.Any() == true)
                {
                    await _ST2136.DeleteResultDetail(existingDetails);
                }

                // Lấy kết quả tổng hợp từ AI
                var criteriaSummaryResults = await _agentCompareService.SummaryResultJson(entity.TextContentAI);
                var criteriaList = criteriaSummaryResults?.Criteria?.ToList();

                if (criteriaList == null || criteriaList.Count == 0)
                    return; // Không có gì để xử lý

                var now = DateTime.Now;
                var statusOk = StatusResultCompare.OK.ToString();
                var statusNg = StatusResultCompare.NG.ToString();
                var statusBlank = StatusResultCompare.BLANK.ToString();

                // Gán thông tin chung + chuẩn hóa status BLANK -> NG
                foreach (var item in criteriaList)
                {
                    item.APK = Guid.NewGuid();
                    item.APKMaster = entity.APK;
                    item.BusinessParent = voucherNo;
                    item.CreateDate = now;
                    item.CreateUserID = entity.CreateUserID;

                    if (item.CriteriaStatus == statusBlank)
                    {
                        item.CriteriaStatus = statusNg;
                    }
                }

                // Lưu chi tiết tiêu chí
                await _ST2136.SaveResultDetail(criteriaList);

                // Lấy các tiêu chí không đạt (khác OK)
                var failedCriteria = criteriaList.Where(x => x.CriteriaStatus != statusOk).ToList();

                if (failedCriteria.Any())
                {
                    var resultDetailText = string.Join(
                        Environment.NewLine,
                        failedCriteria.Select(x => $"Tiêu chí {x.CriteriaID}: {x.CriteriaName} - {x.CriteriaStatus}")
                    );

                    entity.TextConditionFail = resultDetailText;
                    entity.Status = statusNg;
                }
                else
                {
                    entity.Status = statusOk;
                }
                // Cập nhật lại kết quả file
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
   
    }
}
