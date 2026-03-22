using ASOFT.CoreAI.Entities;
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

        private static readonly IReadOnlyList<AgentCriteriaInfo> _criteriaInfos = new[]
         {
            new AgentCriteriaInfo
            {
                Key = AgentCriteriaKeys.CRITERIA_SUPPLIER_NAME,
                Name = "Tên nhà cung cấp"
            },
            new AgentCriteriaInfo
            {
                Key = AgentCriteriaKeys.CRITERIA_INVOICE_NO,
                Name = "Số hóa đơn"
            },
            new AgentCriteriaInfo
            {
                Key = AgentCriteriaKeys.CRITERIA_INVOICE_DATE,
                Name = "Ngày hóa đơn"
            },
            new AgentCriteriaInfo
            {
                Key = AgentCriteriaKeys.CRITERIA_AMOUNT,
                Name = "Số tiền"
            },
            new AgentCriteriaInfo
            {
                Key = AgentCriteriaKeys.CRITERIA_AMOUNT_CUSTOMSHEET,
                Name = "Số tiền tờ khai"
            },
            new AgentCriteriaInfo
            {
                Key = AgentCriteriaKeys.CRITERIA_CURRENCY,
                Name = "Loại tiền"
            },
            new AgentCriteriaInfo
            {
                Key = AgentCriteriaKeys.CRITERIA_PAYMENT_DEADLINE,
                Name = "Hạn thanh toán"
            },
            new AgentCriteriaInfo
            {
                Key = AgentCriteriaKeys.CRITERIA_CHECK_COMPLETED_DATE,
                Name = "Ngày hoàn thành kiểm tra"
            },
            new AgentCriteriaInfo
            {
                Key = AgentCriteriaKeys.CRITERIA_INCOTERM,
                Name = "Điều kiện giao hàng"
            },
            new AgentCriteriaInfo
            {
                Key = AgentCriteriaKeys.CRITERIA_SIGNATURE_STAMP,
                Name = "Chữ ký và Con dấu"
            }
        };
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
        public async Task RunAsync(Guid ST2131APK, ReadFileRequest request, List<ST2130> promptList, CancellationToken ct = default)
        {
            var entity = await _ST2131.GetData(ST2131APK);
            if (entity == null) return;

            try
            {
                ValidateRequest(request);

                // 0. Split file (nếu cần)
                var files = SplitAttachFiles(request.AttachFiles);

                // 1. OCR
                var (ocrText, ocrResults) = await _ocrService.ReadAsync(files, request.BEMF2000ViewModel!.APK);

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
                    _logger.LogError("Không có kết quả tiêu chí được tổng hợp. APK={APK}", ST2131APK);
                    MarkFailed(entity);
                    entity.TextContentOCR = ocrText;
                    await _ST2131.UpdateData(entity);
                    return;
                }

                // 6. Lưu tiêu chí + tổng hợp kết quả
                entity.TextContentOCR = ocrText;

                await _ST2136.SaveData(criteriaList);

                // aiResult hiện tại chưa dùng (bạn đang compare theo từng tiêu chí)
                const string aiResult = "";
                UpdateCompareResult(entity, request, ocrText, aiResult, criteriaList);

                await _ST2131.UpdateData(entity);
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

        private List<AttachFileModel> SplitAttachFiles(List<AttachFileModel>? attachFiles)
        {
            var pageSplit = _settingsManagerService.GetPageSplit().Result;
            var result = new List<AttachFileModel>();
            if (attachFiles == null || attachFiles.Count == 0) return result;

            foreach (var file in attachFiles)
            {
                var split = _filePathService.SplitEveryNPages_KeepOriginal(file, 1000);
                if (split != null && split.Count > 0)
                    result.AddRange(split);
                else
                    result.Add(file);
            }

            return result;
        }

        private async Task<IEnumerable<RedisearchResultItem>?> GetTrainingDataIfNeeded(ReadFileRequest request)
        {
            var configLLM = await _settingsManagerService.GetConfigLLMsAsync();
            if (configLLM != null && configLLM.IsUse) return null;

            return await _trainingService.GetTrainingDataAsync(request, AgentKeys.BEM_AGENT_BEMF2000);
        }

        private async Task<List<AISectionCompare>> BuildAiSectionComparesAsync(IEnumerable<ResultReadFileModel> ocrResults, ST2131 entity)
        {
            var result = new List<AISectionCompare>();
            foreach (var item in ocrResults)
            {
                var formatResult = await FormatOCRIfNeeded(item.TextContent, entity, item.FileName);
                if (formatResult != null && formatResult.Count > 0)
                    result.AddRange(formatResult);
            }

            return result;
        }
        private async Task<List<ST2136>> BuildCriteriaListAsync(ReadFileRequest request, ST2131 entity, List<ST2130> promptList, List<AISectionCompare> aiSectionCompares, List<ResultReadFileModel> ocrResults, CancellationToken ct)
        {
            const string textJson = "";

            var criteriaList = new List<ST2136>();
            int criteriaIndex = 1;
            foreach (var criteriaKey in _criteriaInfos)
            {
                ct.ThrowIfCancellationRequested();

                var promptCriteria = promptList.FirstOrDefault(x => x.TypePrompt == criteriaKey.Key);
                if (promptCriteria == null) continue;

                var aiResultCompare = await _compareService.CompareAsync(
                    request,
                    promptCriteria.Description!,
                    promptCriteria.PromptContent,
                    textJson,
                    ocrResults,
                    aiSectionCompares);

                if (string.IsNullOrWhiteSpace(aiResultCompare)) continue;
                var criteria = _agentCompareService.ParseCriteriaResult(entity, request, aiResultCompare, criteriaIndex, criteriaKey.Name);
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
        private async Task<List<AISectionCompare>?> FormatOCRIfNeeded(string aiResult, ST2131 entity, string fileName)
        {
            var promptReadFile = await _ST2130.GetPromptByCode(AgentKeys.BEM_AGENT_BEMF2000_READFILE);

            if (!string.IsNullOrWhiteSpace(promptReadFile?.PromptContent))
            {
                return await _agentCompareService.FormatOCRText(aiResult, entity, promptReadFile.PromptContent, promptReadFile.Description, fileName);
            }
            return null;
        }
        private static void UpdateCompareResult(ST2131 entity, ReadFileRequest request, string ocrText, string aiResult, List<ST2136> criteriaList)
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
        private static void MarkFailed(ST2131 entity)
        {
            entity.TextConditionFail = string.Empty;
            entity.Status = StatusResultCompare.NG.ToString();
            entity.Percentage = "0%";
            entity.StatusProcess = StatusProcessCompareOCR.FAILED.ToString();
        }
    }
}