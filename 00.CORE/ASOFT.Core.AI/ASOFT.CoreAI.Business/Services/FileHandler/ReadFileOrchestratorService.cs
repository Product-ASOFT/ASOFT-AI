using ASOFT.CoreAI.Business.Services.RedisHandler;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using static ASOFT.CoreAI.Common.AIConstants;

namespace ASOFT.CoreAI.Business
{
    public class ReadFileOrchestratorService
    {
        private readonly IST2130Queries _ST2130Queries;
        private readonly IST2131Queries _ST2131Queries;
        private readonly AgentCompareService _compareService;
        private readonly FilePathService _filePathService;
        private readonly ITrainingDataService _trainingService;
        private readonly IOCRService _ocrService;
        private readonly SettingsManagerService _settingsManager;
        public ReadFileOrchestratorService(IST2130Queries ST2130Queries,
            IST2131Queries ST2131Queries, AgentCompareService compareService,
            FilePathService filePathService, ITrainingDataService trainingDataService,
            IOCRService ocrService, SettingsManagerService settingsManager,
            AgentPromptService agentPromptService)
        {
            _ST2130Queries = ST2130Queries;
            _ST2131Queries = ST2131Queries;
            _compareService = compareService;
            _filePathService = filePathService;
            _trainingService = trainingDataService;
            _ocrService = ocrService;
            _settingsManager = settingsManager;

        }
        public async Task<ChatResponseReadFileModel> HandleAsync(ReadFileRequest request)
        {
            // Validate request
            var (isValid, validationMessage) = ValidateReadFileRequest(request);
            if (!isValid)
            {
                return ChatHandlerHelper.CreateResponseReadFile(validationMessage, false);
            }
            string typeCompare = GetAgentKeyByTypeCompare(request.BEMF2000ViewModel!.PaymentRequestType);
            var prompt = await _ST2130Queries.GetPromptByTypeCompare(AgentKeys.BEM_AGENT_BEMF2000, typeCompare);

            if (prompt == null || string.IsNullOrWhiteSpace(prompt.PromptContent))
                return ChatHandlerHelper.CreateResponseReadFile("Không tồn tại Prompt!", false);

            // Chuẩn hoá file
            var validFiles = _filePathService.NormalizeToPhysicalUnderWebRoot(request.AttachFiles!);
            if (validFiles.Count == 0)
                return ChatHandlerHelper.CreateResponseReadFile("Tệp đính kèm không tồn tại!", false);

            // OCR
            var (ocrText, ocrResults) = await _ocrService.ReadAsync(validFiles, request.BEMF2000ViewModel.APK);
            if (string.IsNullOrWhiteSpace(ocrText))
                return ChatHandlerHelper.CreateResponseReadFile("Không có thông tin đọc được từ tệp đính kèm", false);

            // Training data
            var trainingData = await _trainingService.GetTrainingDataAsync(request, AgentKeys.BEM_AGENT_BEMF2000);

            // Compare via Agent
            var aiResult = await _compareService.CompareAsync(request, prompt.PromptContent!, ocrText, ocrResults, trainingData);

            // Build entity
            var entity = new ST2131
            {
                APK = Guid.NewGuid(),
                APKMaster = request.BEMF2000ViewModel.APK,
                AttachName = "Thông tin kết quả đối chiếu",
                CreateUserID = request.UserId,
                CreateDate = DateTime.Now,
                TextContentOCR = ocrText,
                DivisionID = request.BEMF2000ViewModel.DivisionID,
                AttachID = validFiles.Select(x => x.AttachID).FirstOrDefault(),
                TextContentAI = !string.IsNullOrWhiteSpace(aiResult) ? aiResult : "Không có kết quả đối chiếu",
            };

            var match = ExtractMatchInfo.Extract(aiResult);
            if (!string.IsNullOrEmpty(match.MatchRate)) entity.Percentage = match.MatchRate;
            if (!string.IsNullOrEmpty(match.Conclusion)) entity.Status = match.Conclusion;

            // Save
            var resultSaveFile = await _ST2131Queries.SaveFileResult(entity);
            return ChatHandlerHelper.CreateResponseReadFile(
                resultSaveFile ? "Đọc và ghi kết quả thành công" :
                "Đọc và ghi kết quả không thành công", resultSaveFile);

        }
        public async Task<List<ResultReadFileModel>> ReadFileFromChatBot(List<string> FilePaths, Guid APK)
        {
            string configKeyOCR = _settingsManager.GetKeyReadOCR();
            var AttachFiles = new List<AttachFileModel>();
            foreach (var item in FilePaths)
            {
                AttachFiles.Add(new AttachFileModel
                {
                    AttachURL = item,
                    AttachName = Path.GetFileName(item)
                });
            }
            if (FilePaths != null && FilePaths.Any())
            {
                var (ocrText, results) = await _ocrService.ReadAsync(AttachFiles, APK);
                return results;
            }
            return new List<ResultReadFileModel>();
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
        private (bool IsValid, string Message) ValidateReadFileRequest(ReadFileRequest request)
        {
            if (request == null)
                return (false, "Thông tin dữ liệu không được để trống!");

            if (string.IsNullOrWhiteSpace(request.UserId))
                return (false, "Thông tin người dùng không được để trống!");

            if (request.BEMF2000ViewModel == null)
                return (false, "Chưa có thông tin về phiếu DNTT");

            if (string.IsNullOrWhiteSpace(request.BEMF2000ViewModel.PaymentRequestType))
                return (false, "Chưa có thông tin về loại phiếu DNTT cần đối chiếu!");

            if (request.AttachFiles == null || !request.AttachFiles.Any(x => !string.IsNullOrWhiteSpace(x.AttachURL)))
                return (false, "Tệp đính kèm không tồn tại!");

            return (true, string.Empty);
        }
    }
}
