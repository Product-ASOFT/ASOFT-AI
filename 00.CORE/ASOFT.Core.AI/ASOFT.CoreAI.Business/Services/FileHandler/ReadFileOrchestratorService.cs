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
            if (request == null)
                return ChatHandlerHelper.CreateResponseReadFile("Request body is null.", false);

            if (string.IsNullOrWhiteSpace(request.UserId))
                return ChatHandlerHelper.CreateResponseReadFile("UserId is required.", false);

            if (request.BEMF2002Detail == null)
                return ChatHandlerHelper.CreateResponseReadFile("BEMF2002Detail is required.", false);

            if (request.AttachFiles == null || !request.AttachFiles.Any(x => !string.IsNullOrWhiteSpace(x.AttachURL)))
                return ChatHandlerHelper.CreateResponseReadFile("Invalid request or file path is empty.", false);


            var prompt = await _ST2130Queries.QueryPromptsByAgentCode(AgentKeys.BEM_AGENT_BEMF2000);
            if (prompt == null || string.IsNullOrWhiteSpace(prompt.PromptContent))
                return ChatHandlerHelper.CreateResponseReadFile("Không tồn tại Prompt!", false);

            // Chuẩn hoá file
            var validFiles = _filePathService.NormalizeToPhysicalUnderWebRoot(request.AttachFiles);
            if (validFiles.Count == 0)
                return ChatHandlerHelper.CreateResponseReadFile("No valid file paths for OCR.", false);

            // OCR
            var (ocrText, ocrResults) = await _ocrService.ReadAsync(validFiles);
            if (string.IsNullOrWhiteSpace(ocrText))
                return ChatHandlerHelper.CreateResponseReadFile("No text extracted from the file.", false);

            // Training data
            var trainingData = await _trainingService.GetTrainingDataAsync(request, AgentKeys.BEM_AGENT_BEMF2000);

            // Compare via Agent
            var aiResult = await _compareService.CompareAsync(request, prompt.PromptContent!, ocrText, ocrResults, trainingData);

            // Build entity
            var entity = new ST2131
            {
                APK = Guid.NewGuid(),
                APKMaster = request.BEMF2002Detail.APK,
                AttachName = "Thông tin kết quả đối chiếu",
                CreateUserID = request.UserId,
                CreateDate = DateTime.Now,
                TextContentOCR = ocrText,
                DivisionID = request.BEMF2002Detail.DivisionID,
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
        public async Task<List<ResultReadFileModel>> ReadFileFromChatBot(List<string> FilePaths)
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
                var (ocrText, results) = await _ocrService.ReadAsync(AttachFiles);
                return results;
            }
            return new List<ResultReadFileModel>();
        }

    }
}
