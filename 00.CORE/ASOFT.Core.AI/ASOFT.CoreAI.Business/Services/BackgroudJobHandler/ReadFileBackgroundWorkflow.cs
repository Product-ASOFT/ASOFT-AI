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
        private readonly SettingsManagerService _settingsManagerService;
        private readonly FilePathService _filePathService;
        public ReadFileBackgroundWorkflow(
            IST2131Queries ST2131,
            IST2130Queries ST2130,
            IST2136Queries ST2136,
            IOCRService ocrService,
            ITrainingDataService trainingService,
            AgentCompareService compareService,
            ILogger<ReadFileBackgroundWorkflow> logger,
            AgentCompareService agentCompareService,
            SettingsManagerService settingsManagerService,
            FilePathService filePathService)
        {
            _ST2131 = ST2131;
            _ST2130 = ST2130;
            _ST2136 = ST2136;
            _ocrService = ocrService;
            _trainingService = trainingService;
            _compareService = compareService;
            _logger = logger;
            _agentCompareService = agentCompareService;
            _settingsManagerService = settingsManagerService;
            _filePathService = filePathService;
        }
        public async Task RunAsync(Guid ST2131APK, ReadFileRequest request, string promptSytem, string promptContent, CancellationToken ct = default)
        {
            var entity = await _ST2131.GetData(ST2131APK);
            if (entity == null) return;

            try
            {
                ValidateRequest(request);
                var filePathsSplit = new List<AttachFileModel>();
                foreach (var item in request.AttachFiles!)
                {
                    var split = _filePathService.SplitEveryNPages_KeepOriginal(item);
                    if (split != null && split.Any())
                    {
                        filePathsSplit.AddRange(split);
                    }
                    else
                    {
                        filePathsSplit.Add(item);
                    }
                }

                // 1. OCR
                var (ocrText, ocrResults) = await _ocrService.ReadAsync(filePathsSplit, request.BEMF2000ViewModel!.APK);

                if (string.IsNullOrWhiteSpace(ocrText))
                    throw new Exception("Không có thông tin đọc được từ tệp đính kèm");

                // 2. Training data
                var configLLM = await _settingsManagerService.GetConfigLLMsAsync();
                IEnumerable<RedisearchResultItem>? trainingData = null;
                if (configLLM == null || configLLM.IsUse == false)
                {
                    trainingData = await _trainingService.GetTrainingDataAsync(request, AgentKeys.BEM_AGENT_BEMF2000);
                }
                // Dữ liệu được lấy từ file đính kèm
                string textJson = string.Empty;
                foreach (var item in ocrResults)
                {
                    string text = item.FileName + item.TextContent;
                    //7.Format OCR Text(nếu có prompt)
                    textJson += await FormatOCRIfNeeded(text, entity);
                }

                //string aiResult = await _compareService.CompareAsync(request,promptSytem, promptContent, ocrText, ocrResults, trainingData);
                string aiResult = await _compareService.CompareAsync(request, promptSytem, promptContent, textJson, ocrResults, trainingData);

                #region 3. Compare AI comment lại tiến trình xử lý song song
                //// Khởi tạo task song song
                //Task<string> compareTask = _compareService.CompareAsync(request, promptContent, ocrText, ocrResults, trainingData);

                //Task formatTask = string.IsNullOrWhiteSpace(promptContent) ? Task.CompletedTask : FormatOCRIfNeeded(ocrText, entity);

                //// Chờ cả 2 hoàn thành
                //await Task.WhenAll(compareTask, formatTask);

                ////Lấy kết quả AI
                //string aiResult = await compareTask;
                #endregion
                entity.TextContentOCR = ocrText;
                entity.TextContentAI = aiResult;
                // 4. Parse & xử lý kết quả tiêu chí
                var criteriaList = await BuildCriteriaList(entity, request, aiResult);
                if (!criteriaList.Any())
                {
                    _logger.LogError("Không có kết quả được tổng hợp");
                    MarkFailed(entity);
                    await _ST2131.UpdateData(entity);
                    return;
                }
                // 5. Lưu chi tiết tiêu chí
                await _ST2136.SaveData(criteriaList);

                // 6. Tổng hợp kết quả
                UpdateCompareResult(entity, request, ocrText, aiResult, criteriaList);

                await _ST2131.UpdateData(entity);

                ////7.Format OCR Text(nếu có prompt)
                //await FormatOCRIfNeeded(ocrText, entity);
            }
            catch (OperationCanceledException)
            {
                MarkFailed(entity);
                await _ST2131.UpdateData(entity);
            }
            catch (Exception ex)
            {
                MarkFailed(entity);
                await _ST2131.UpdateData(entity);
                _logger.LogError(ex, "ReadFile job failed for {APK}", ST2131APK);
            }
        }
        private void ValidateRequest(ReadFileRequest request)
        {
            if (request == null)
                throw new Exception("Không tìm thấy request.");
        }
        private async Task<string> FormatOCRIfNeeded(string aiResult, ST2131 entity)
        {
            var promptReadFile = await _ST2130.GetPromptByCode(AgentKeys.BEM_AGENT_BEMF2000_READFILE);

            if (!string.IsNullOrWhiteSpace(promptReadFile?.PromptContent))
            {
                return await _agentCompareService.FormatOCRText(aiResult, entity, promptReadFile.PromptContent, promptReadFile.Description);
            }
            return string.Empty;
        }
        private async Task<List<ST2136>> BuildCriteriaList(ST2131 entity, ReadFileRequest request, string aiResult)
        {
            var summary = await _agentCompareService.SummaryResultJson(aiResult);
            var criteriaList = summary?.Criteria?.ToList() ?? new();

            if (!criteriaList.Any()) return criteriaList;

            var now = DateTime.Now;
            var voucherNo = request.BEMF2000ViewModel?.VoucherNo ?? string.Empty;

            foreach (var item in criteriaList)
            {
                item.APK = Guid.NewGuid();
                item.APKMaster = entity.APK;
                item.BusinessParent = voucherNo;
                item.CreateDate = now;
                item.CreateUserID = entity.CreateUserID;

                // Chuẩn hóa BLANK -> NG
                if (item.CriteriaStatus == StatusResultCompare.BLANK.ToString())
                {
                    item.CriteriaStatus = StatusResultCompare.NG.ToString();
                }
            }
            return criteriaList;
        }
        private static void UpdateCompareResult(ST2131 entity, ReadFileRequest request, string ocrText, string aiResult, List<ST2136> criteriaList)
        {
            var statusOk = StatusResultCompare.OK.ToString();
            var statusNg = StatusResultCompare.NG.ToString();

            entity.TextContentOCR = ocrText;
            entity.AttachID = request.AttachFiles!.Select(x => x.AttachID).FirstOrDefault();
            entity.TextContentAI = string.IsNullOrWhiteSpace(aiResult)
                ? "Không có kết quả đối chiếu"
                : aiResult;
            entity.StatusProcess = StatusProcessCompareOCR.COMPLETED.ToString();

            var failedCriteria = criteriaList
                .Where(x => x.CriteriaStatus != statusOk)
                .ToList();

            if (failedCriteria.Any())
            {
                entity.TextConditionFail = string.Join(
                    Environment.NewLine,
                    failedCriteria.Select(x => $"Tiêu chí {x.CriteriaID}: {x.CriteriaName} - {x.CriteriaStatus}"));

                var total = criteriaList.Count;
                var failed = failedCriteria.Count;
                var percentage = total > 0 ? (double)(total - failed) / total * 100 : 0;

                entity.Percentage = $"{percentage:0.00}%";
                entity.Status = statusNg;
            }
            else
            {
                entity.TextConditionFail = string.Empty;
                entity.Percentage = "100%";
                entity.Status = statusOk;
            }
        }
        private static void MarkFailed(ST2131 entity)
        {
            entity.TextConditionFail = string.Empty;
            entity.Status = StatusResultCompare.NG.ToString();
            entity.Percentage = "0%";
            entity.StatusProcess = StatusProcessCompareOCR.FAILED.ToString();
        }
    }
}