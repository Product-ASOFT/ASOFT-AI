using ASOFT.A00.Entities;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Entities.ViewModels.AI;
using ASOFT.CoreAI.Infrastructure;
using ASOFT.CoreAI.Infrastructure.Interface;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using static ASOFT.CoreAI.Common.AIConstants;
using static ASOFT.CoreAI.Common.EnumConstants;

namespace ASOFT.CoreAI.Business
{
    public sealed class ReadFileBackgroundWorkflow : IReadFileBackgroundWorkflow
    {
        private readonly IBEMT2003Queries _BEMT2003;
        private readonly IONT1042Queries _ONT1042;
        private readonly IBEMT2004Queries _BEMT2004;
        private readonly IOOT9002Queries _OOT9002;
        private readonly IOOT9003Queries _OOT9003;

        private readonly ITrainingDataService _trainingService;
        private readonly IOCRService _ocrService;
        private readonly AgentCompareService _compareService;
        private readonly ILogger<ReadFileBackgroundWorkflow> _logger;
        private readonly AgentCompareService _agentCompareService;
        private readonly SettingsManagerService _settingsManagerService;

        public ReadFileBackgroundWorkflow(
            IBEMT2003Queries BEMT2003,
            IONT1042Queries ONT1042,
            IBEMT2004Queries BEMT2004,
            IOOT9002Queries OOT9002,
            IOOT9003Queries OOT9003,
            IOCRService ocrService,
            ITrainingDataService trainingService,
            AgentCompareService compareService,
            ILogger<ReadFileBackgroundWorkflow> logger,
            AgentCompareService agentCompareService,
            SettingsManagerService settingsManagerService)
        {
            _BEMT2003 = BEMT2003;
            _ONT1042 = ONT1042;
            _BEMT2004 = BEMT2004;
            _OOT9002 = OOT9002;
            _OOT9003 = OOT9003;
            _ocrService = ocrService;
            _trainingService = trainingService;
            _compareService = compareService;
            _logger = logger;
            _agentCompareService = agentCompareService;
            _settingsManagerService = settingsManagerService;
        }
        public async Task RunAsync(Guid BEMT2003APK, ReadFileRequest request, List<PromptContentViewModel> promptList, CancellationToken ct = default)
        {
            var entity = await _BEMT2003.GetData(BEMT2003APK);
            if (entity == null) return;

            try
            {
                ValidateRequest(request);
                // 0. Split file (nếu cần)
                //var files = SplitAttachFiles(request.AttachFiles);

                // 1. OCR
                var (ocrText, ocrResults) = await _ocrService.ReadAsync(request.AttachFiles!, request.BEMF2000ViewModel!.APK);

                if (string.IsNullOrWhiteSpace(ocrText))
                    throw new Exception("Không có thông tin đọc được từ tệp đính kèm");

                // 2. Training data 
                var trainingData = await GetTrainingDataIfNeeded(request);

                // 3. Build AI sections từ OCR (dùng cho compare)
                var aiSectionCompares = await BuildAiSectionComparesAsync(ocrResults, entity);

                // 4. Build criteria list
                var criteriaList = await BuildCriteriaListAsync(request: request, entity: entity, promptList: promptList, aiSectionCompares: aiSectionCompares, ocrResults: ocrResults, ct: ct);

                // 5. Nếu không có kết quả -> fail
                if (criteriaList.Count == 0)
                {
                    _logger.LogError("Không có kết quả tiêu chí được tổng hợp. APK={APK}", BEMT2003APK);
                    MarkFailed(entity);
                    entity.TextContentOCR = ocrText;
                    await _BEMT2003.UpdateData(entity);
                    await NotificationResultAI(request);
                    return;
                }

                // 6. Lưu tiêu chí + tổng hợp kết quả
                entity.TextContentOCR = ocrText;

                await _BEMT2004.SaveData(criteriaList);

                // aiResult hiện tại chưa dùng (bạn đang compare theo từng tiêu chí)
                const string aiResult = "";
                UpdateCompareResult(entity, request, ocrText, aiResult, criteriaList);

                await _BEMT2003.UpdateData(entity);
                await NotificationResultAI(request);
            }
            catch (OperationCanceledException)
            {
                MarkFailed(entity);
                await _BEMT2003.UpdateData(entity);
                await NotificationResultAI(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReadFile job failed for {APK}", BEMT2003APK);
                MarkFailed(entity);
                await _BEMT2003.UpdateData(entity);
                await NotificationResultAI(request);
            }
        }

        private async Task<IEnumerable<RedisearchResultItem>?> GetTrainingDataIfNeeded(ReadFileRequest request)
        {
            var configLLM = await _settingsManagerService.GetConfigLLMsAsync();
            if (configLLM != null && configLLM.IsUse) return null;

            return await _trainingService.GetTrainingDataAsync(request, AgentKeys.BEM_AGENT_BEMF2000);
        }

        private async Task<List<Dictionary<string, object?>>> BuildAiSectionComparesAsync(IEnumerable<ResultReadFileModel> ocrResults, BEMT2003 entity)
        {
            var result = new List<Dictionary<string, object?>>();
            foreach (var item in ocrResults)
            {
                var formatResult = await FormatOCRIfNeeded(item.TextContent, entity, item.FileName);
                if (formatResult != null && formatResult.Count > 0)
                    result.AddRange(formatResult);
            }

            return result;
        }
        private async Task<List<BEMT2004>> BuildCriteriaListAsync(ReadFileRequest request, BEMT2003 entity, List<PromptContentViewModel> promptList, List<Dictionary<string, object?>> aiSectionCompares, List<ResultReadFileModel> ocrResults, CancellationToken ct)
        {
            const string textJson = "";

            var criteriaList = new List<BEMT2004>();
            int criteriaIndex = 1;
            foreach (var prompt in promptList)
            {
                var aiResultCompare = await _compareService.CompareAsync(
                    request,
                    prompt.PromptSystem,
                    prompt.PromptUser,
                    textJson,
                    ocrResults,
                    aiSectionCompares);

                if (string.IsNullOrWhiteSpace(aiResultCompare)) continue;
                var criteria = _agentCompareService.ParseCriteriaResult(entity, request, aiResultCompare, criteriaIndex, prompt.CriteriaName);
                if (criteria != null)
                {
                    criteriaIndex++;
                    criteriaList.Add(criteria);
                }
            }

            return criteriaList;
        }
        private void ValidateRequest(ReadFileRequest request)
        {
            if (request == null)
                throw new Exception("Không tìm thấy request.");
        }
        private async Task<List<Dictionary<string, object?>>?> FormatOCRIfNeeded(string aiResult, BEMT2003 entity, string fileName)
        {
            var typeCase = 2; // case map từ ONT1040 với ONT1042
            var dataPrompt = await _ONT1042.GetDataPrompt(typeCase, string.Empty, AgentKeys.BEM_AGENT_READFILE);

            if (dataPrompt.Any())
            {
                var promptContent = dataPrompt.FirstOrDefault();
                return await _agentCompareService.FormatOCRText(aiResult, entity, promptContent.PromptUser, promptContent.PromptSystem, fileName);
            }
            return null;
        }
        private static void UpdateCompareResult(BEMT2003 entity, ReadFileRequest request, string ocrText, string aiResult, List<BEMT2004> criteriaList)
        {
            var statusOk = StatusResultCompare.OK.ToString();
            var statusNg = StatusResultCompare.NG.ToString();

            entity.TextContentOCR = ocrText;
            entity.AttachID = request.AttachFiles!.Select(x => x.AttachID).FirstOrDefault();
            //entity.TextContentAI = string.IsNullOrWhiteSpace(aiResult)
            //    ? "Không có kết quả đối chiếu"
            //    : aiResult;
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
        private static void MarkFailed(BEMT2003 entity)
        {
            entity.TextConditionFail = string.Empty;
            entity.Status = StatusResultCompare.NG.ToString();
            entity.Percentage = "0%";
            entity.StatusProcess = StatusProcessCompareOCR.FAILED.ToString();
        }
        private async Task NotificationResultAI(ReadFileRequest request)
        {
            if (request?.OOT9002 == null || request.OOT9003s == null || !request.OOT9003s.Any())
                return;

            var now = DateTime.Now;

            var createUserId = request.BEMF2001ViewModels?
                .Select(x => x.CreateUserID)
                .FirstOrDefault();


            var voucherNo = request.BEMF2000ViewModel?.VoucherNo ?? string.Empty;

            var message = $"Phiếu DNTT/DNTTTU/DNTU {voucherNo} đã hoàn tất đối chiếu";

            var notify = request.OOT9002;

            notify.Title = message;
            notify.Description = message;

            notify.CreateDate = now;
            notify.LastModifyDate = now;

            notify.CreateUserID = createUserId;
            notify.LastModifyUserID = createUserId;

            notify.EffectDate = now;
            notify.ExpiryDate = now;

            // 3. Map OOT9003
            foreach (var item in request.OOT9003s)
            {
                item.CreateDate = now;
                item.LastModifyDate = now;

                item.CreateUserID = createUserId;
                item.LastModifyUserID = createUserId;
            }

            // 4. Save DB 
            bool IsSaveOOT9002 = await _OOT9002.SaveData(notify);
            bool IsSaveOOT9003 = await _OOT9003.SaveData(request.OOT9003s);
            if (IsSaveOOT9002 && IsSaveOOT9003)
            {
                // 5. Call API notify client
                await SendNotificationToClientAsync();
            }
        }
        private async Task SendNotificationToClientAsync()
        {
            var baseUrl = await _settingsManagerService.GetUrlERPAsync();

            if (string.IsNullOrWhiteSpace(baseUrl))
                return;

            const string tokenRequest = "hkv156jcbhkjvcbKJlkjbvSAHG8D4521VX12C234AG4574JSVB456bhdgfs78214OFidugjvmkjbvcbcvhjdfgjkbcnmhg7675JHBJHBVBSV6JHJHj7sdfj32156465431ksf";

            using var httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };

            var url = $"Notification/SendToClient?tokenRequest={Uri.EscapeDataString(tokenRequest)}";

            using var response = await httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();
        }
    }
}