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
        private readonly IST2136Queries _ST2136Queries;
        private readonly IST2137Queries _ST2137Queries;
        private readonly IST2138Queries _ST2138Queries;
        private readonly IOCRService _ocrService;
        private readonly SettingsManagerService _settingsManager;
        private readonly IJobQueue _jobQueue;

        public ReadFileOrchestratorService(IST2130Queries ST2130Queries,
            IST2131Queries ST2131Queries, IST2136Queries ST2136Queries,
            IST2137Queries ST2137Queries, IST2138Queries ST2138Queries,
            IOCRService ocrService, SettingsManagerService settingsManager,
            AgentPromptService agentPromptService, IJobQueue jobQueue)
        {
            _ST2130Queries = ST2130Queries;
            _ST2131Queries = ST2131Queries;
            _ST2136Queries = ST2136Queries;
            _ST2137Queries = ST2137Queries;
            _ST2138Queries = ST2138Queries;
            _ocrService = ocrService;
            _settingsManager = settingsManager;
            _jobQueue = jobQueue;
        }
        // Hàm xử lý chính đọc file và đối chiếu
        public async Task<ChatResponseReadFileModel> HandleAsync(ReadFileRequest request)
        {
            // 1) Validate
            var (isValid, validationMessage) = ValidateReadFileRequest(request);
            if (!isValid)
                return ChatHandlerHelper.CreateResponseReadFile(validationMessage, false);

            // Xóa dữ liệu đối chiếu trước đó 
            await DeleteData(request.BEMF2000ViewModel!);

            //// 2) (Tuỳ chọn) Kiểm tra tồn tại prompt sớm để fail fast (hoặc để workflow kiểm tra)
            string typeCompare = request.BEMF2000ViewModel!.PaymentRequestTypeID;
            var agentPrompts = await _ST2130Queries.GetPromptsByAgentCodeAndTypeCompare(AgentKeys.BEM_AGENT_BEMF2000, typeCompare);
            if (agentPrompts == null || !agentPrompts.Any())
                return ChatHandlerHelper.CreateResponseReadFile("Chưa thiết lập thông tin prompt!", false);

            // 3) Tạo record PROCESSING
            var entity = new ST2131
            {
                APK = Guid.NewGuid(),
                APKMaster = request.BEMF2000ViewModel.APK,
                AttachName = "Thông tin kết quả đối chiếu",
                CreateUserID = request.BEMF2000ViewModel.CreateUserID,
                CreateDate = DateTime.Now,
                DivisionID = request.BEMF2000ViewModel.DivisionID,
                StatusProcess = StatusProcessCompareOCR.PROCESSING.ToString(),
            };

            var saved = await _ST2131Queries.SaveData(entity);
            if (!saved)
                return ChatHandlerHelper.CreateResponseReadFile("Không thể khởi tạo lưu kết quả.", false);

            // 4) Đẩy job chạy nền (worker sẽ tự OCR → compare → cập nhật)
            await _jobQueue.EnqueueAsync(new ReadFileJob(entity.APK, request, agentPrompts));

            // 5) Trả về ngay cho UI
            return ChatHandlerHelper.CreateResponseReadFile($"Đã nhận yêu cầu. Mã kết quả: {request.BEMF2000ViewModel.VoucherNo}. Hệ thống đang xử lý nền.", true);
        }
        // Hàm đọc file từ chatbot
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
        // Hàm lấy key agent theo loại đối chiếu
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
        // Hàm validate request đọc file
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

            if (string.IsNullOrWhiteSpace(request.BEMF2000ViewModel.PaymentRequestTypeID))
                return (false, "Chưa có thông tin về loại phiếu DNTT cần đối chiếu!");

            if (request.AttachFiles == null || !request.AttachFiles.Any(x => !string.IsNullOrWhiteSpace(x.AttachURL)))
                return (false, "Tệp đính kèm không tồn tại!");

            return (true, string.Empty);
        }
        // Hàm xóa dữ liệu đối chiếu cũ (nếu có)
        private async Task DeleteData(BEMF2000ViewModel BEMF2000ViewModel)
        {
            try
            {
                var ST2131_Delete = await _ST2131Queries.GetDataByAPKMaster(BEMF2000ViewModel.APK);
                if (ST2131_Delete == null)
                {
                    return;
                }
                await _ST2137Queries.DeleteData(ST2131_Delete.APK); // Xóa dữ liệu bảng master dữ liệu đọc từ file đính kèm
                await _ST2138Queries.DeleteData(ST2131_Delete.APK); // Xóa dữ liệu bảng detail dữ liệu đọc từ file đính kèm
                await _ST2136Queries.DeleteData(ST2131_Delete.APK); // Xóa dữ liệu bảng chi tiết  kết quả đối chiếu từ AI
                await _ST2131Queries.DeleteData(ST2131_Delete); // Xóa dữ liệu bảng chính kết quả đối chiếu từ AI
            }
            catch (Exception)
            {
                return;
            }
            return;
        }

    }
}