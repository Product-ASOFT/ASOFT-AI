using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using ASOFT.CoreAI.Infrastructure.Interface;
using static ASOFT.CoreAI.Common.AIConstants;
using static ASOFT.CoreAI.Common.EnumConstants;

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
        private readonly IJobQueue _jobQueue;

        public ReadFileOrchestratorService(IST2130Queries ST2130Queries,
            IST2131Queries ST2131Queries, AgentCompareService compareService,
            FilePathService filePathService, ITrainingDataService trainingDataService,
            IOCRService ocrService, SettingsManagerService settingsManager,
            AgentPromptService agentPromptService, IJobQueue jobQueue)
        {
            _ST2130Queries = ST2130Queries;
            _ST2131Queries = ST2131Queries;
            _compareService = compareService;
            _filePathService = filePathService;
            _trainingService = trainingDataService;
            _ocrService = ocrService;
            _settingsManager = settingsManager;
            _jobQueue = jobQueue;
        }

        public async Task<ChatResponseReadFileModel> HandleAsync(ReadFileRequest request)
        {
            // 1) Validate
            var (isValid, validationMessage) = ValidateReadFileRequest(request);
            if (!isValid)
                return ChatHandlerHelper.CreateResponseReadFile(validationMessage, false);

            // 2) (Tuỳ chọn) Kiểm tra tồn tại prompt sớm để fail fast (hoặc để workflow kiểm tra)
            string typeCompare = GetAgentKeyByTypeCompare(request.BEMF2000ViewModel!.PaymentRequestType);
            var prompt = await _ST2130Queries.GetPromptByTypeCompare(AgentKeys.BEM_AGENT_BEMF2000, typeCompare);
            if (prompt == null || string.IsNullOrWhiteSpace(prompt.PromptContent))
                return ChatHandlerHelper.CreateResponseReadFile("Không tồn tại Prompt!", false);

            // 3) Tạo record PROCESSING
            var entity = new ST2131
            {
                APK = Guid.NewGuid(),
                APKMaster = request.BEMF2000ViewModel.APK,
                AttachName = "Thông tin kết quả đối chiếu",
                CreateUserID = request.UserId,
                CreateDate = DateTime.Now,
                DivisionID = request.BEMF2000ViewModel.DivisionID,
                StatusProcess = StatusProcessCompareOCR.PROCESSING.ToString(),
            };

            var saved = await _ST2131Queries.SaveData(entity);
            if (!saved)
                return ChatHandlerHelper.CreateResponseReadFile("Không thể khởi tạo lưu kết quả.", false);

            // 4) Đẩy job chạy nền (worker sẽ tự OCR → compare → cập nhật)
            await _jobQueue.EnqueueAsync(new ReadFileJob(entity.APK, request, prompt.PromptContent));

            // 5) Trả về ngay cho UI
            return ChatHandlerHelper.CreateResponseReadFile($"Đã nhận yêu cầu. Mã kết quả: {entity.APK}. Hệ thống đang xử lý nền.", true);
        }

        public async Task<List<ResultReadFileModel>> ReadFileFromChatBot(List<string> FilePaths, Guid APK)
        {
            string configKeyOCR = await _settingsManager.GetKeyReadOCRAsync();
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
                "WAREHOUSE" => AgentTypeKeys.BEM_AGENT_BEMF2000_WAREHOUSE,
                "MACHINE" => AgentTypeKeys.BEM_AGENT_BEMF2000_MACHINE,
                "SERVICE" => AgentTypeKeys.BEM_AGENT_BEMF2000_SERVICE,
                "BUILD" => AgentTypeKeys.BEM_AGENT_BEMF2000_BUILD,
                "OTHER" => AgentTypeKeys.BEM_AGENT_BEMF2000_OTHER,
                _ => throw new NotImplementedException(),
            };
        }

        private (bool IsValid, string Message) ValidateReadFileRequest(ReadFileRequest request)
        {
            var CheckConfigModelAI = _settingsManager.CheckConfigModelAI().Result;
            if (CheckConfigModelAI.Status == false)
                return (false, CheckConfigModelAI.Result);

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