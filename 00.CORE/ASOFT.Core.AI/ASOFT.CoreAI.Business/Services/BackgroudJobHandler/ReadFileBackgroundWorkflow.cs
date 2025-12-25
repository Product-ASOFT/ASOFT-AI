using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using ASOFT.CoreAI.Infrastructure.Interface;
using Microsoft.Extensions.Logging;
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
            var entity = await _ST2131.GetData(ST2131APK);
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

                var promptReadFile = await _ST2130.GetPromptByCode(AgentKeys.BEM_AGENT_BEMF2000_READFILE);
                if (promptReadFile != null && !string.IsNullOrWhiteSpace(promptReadFile.PromptContent))
                {
                    await _agentCompareService.FormatOCRText(aiResult, entity, promptReadFile.PromptContent);
                }
                
                // Lấy kết quả tổng hợp từ AI
                var criteriaSummaryResults = await _agentCompareService.SummaryResultJson(aiResult);
                var criteriaList = criteriaSummaryResults?.Criteria?.ToList();

                if (criteriaList == null || criteriaList.Count == 0)
                    return; // Không có gì để xử lý

                // Update kết quả
                entity.TextContentOCR = ocrText;
                entity.AttachID = request.AttachFiles!.Select(x => x.AttachID).FirstOrDefault();
                entity.TextContentAI = !string.IsNullOrWhiteSpace(aiResult) ? aiResult : "Không có kết quả đối chiếu";
                entity.StatusProcess = StatusProcessCompareOCR.COMPLETED.ToString();

                var voucherNo = request.BEMF2000ViewModel.VoucherNo ?? string.Empty;
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
                await _ST2136.SaveData(criteriaList);

                // Lấy các tiêu chí không đạt (khác OK)
                var failedCriteria = criteriaList.Where(x => x.CriteriaStatus != statusOk).ToList();
                int numberCritera = criteriaList.Count();
                if (failedCriteria.Any())
                {
                    var resultDetailText = string.Join(
                        Environment.NewLine,
                        failedCriteria.Select(x => $"Tiêu chí {x.CriteriaID}: {x.CriteriaName} - {x.CriteriaStatus}")
                    );
                    double numberNG = failedCriteria.Count();
                    double percentage = 0;
                    if (numberCritera > 0)
                    {
                        percentage = (numberCritera - numberNG) / numberCritera * 100;
                    }
                    entity.Percentage = string.Format("{0}%", percentage.ToString("0.00"));

                    entity.TextConditionFail = resultDetailText;
                    entity.Status = statusNg;
                }
                else
                {
                    entity.Percentage = "100%";
                    entity.Status = statusOk;
                }
                // Cập nhật lại kết quả file
                await _ST2131.UpdateData(entity);
            }
            catch (OperationCanceledException)
            {
                entity.Status = StatusResultCompare.NG.ToString();
                entity.Percentage = "0%";
                entity.StatusProcess = StatusProcessCompareOCR.FAILED.ToString();
                await _ST2131.UpdateData(entity);
            }
            catch (Exception ex)
            {
                entity.Status = StatusResultCompare.NG.ToString();
                entity.Percentage = "0%";
                entity.StatusProcess = StatusProcessCompareOCR.FAILED.ToString();
                await _ST2131.UpdateData(entity);
                _logger.LogError(ex, "ReadFile job failed for {APK}", ST2131APK);
            }
        }
    }
}