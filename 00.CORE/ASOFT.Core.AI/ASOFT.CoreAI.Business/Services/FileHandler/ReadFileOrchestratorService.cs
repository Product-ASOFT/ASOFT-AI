using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using ASOFT.CoreAI.Infrastructure.Interface;
using static ASOFT.CoreAI.Common.AIConstants;
using static ASOFT.CoreAI.Common.EnumConstants;

namespace ASOFT.CoreAI.Business
{
    public class ReadFileOrchestratorService
    {
        private readonly IONT1042Queries _ONT1042Queries;
        private readonly IBEMT2003Queries _BEMT2003Queries;
        private readonly IBEMT2004Queries _BEMT2004Queries;
        private readonly IBEMT2005Queries _BEMT2005Queries;
        private readonly IBEMT2006Queries _BEMT2006Queries;
        private readonly IOCRService _ocrService;
        private readonly SettingsManagerService _settingsManager;
        private readonly IJobQueue _jobQueue;

        public ReadFileOrchestratorService(IONT1042Queries ONT1042Queries,
            IBEMT2003Queries BEMT2003Queries, IBEMT2004Queries BEMT2004Queries,
            IBEMT2005Queries BEMT2005Queries, IBEMT2006Queries BEMT2006Queries,
            IOCRService ocrService, SettingsManagerService settingsManager,
            AgentPromptService agentPromptService, IJobQueue jobQueue)
        {
            _ONT1042Queries = ONT1042Queries;
            _BEMT2003Queries = BEMT2003Queries;
            _BEMT2004Queries = BEMT2004Queries;
            _BEMT2005Queries = BEMT2005Queries;
            _BEMT2006Queries = BEMT2006Queries;
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
            int caseType = 1; // 1 - sử dụng để phân loại đối chiếu theo tiêu chí
            var agentPrompts = await _ONT1042Queries.GetDataPrompt(caseType, typeCompare, string.Empty);
            if (agentPrompts == null || !agentPrompts.Any())
                return ChatHandlerHelper.CreateResponseReadFile("Chưa thiết lập thông tin prompt!", false);

            // 3) Tạo record PROCESSING
            var entity = new BEMT2003
            {
                APK = Guid.NewGuid(),
                APKMaster = request.BEMF2000ViewModel.APK,
                AttachName = "Thông tin kết quả đối chiếu",
                CreateUserID = request.BEMF2000ViewModel.CreateUserID,
                CreateDate = DateTime.Now,
                DivisionID = request.BEMF2000ViewModel.DivisionID,
                StatusProcess = StatusProcessCompareOCR.PROCESSING.ToString(),
            };

            var saved = await _BEMT2003Queries.SaveData(entity);
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
                var BEMT2003_Delete = await _BEMT2003Queries.GetDataByAPKMaster(BEMF2000ViewModel.APK);
                if (BEMT2003_Delete == null)
                {
                    return;
                }
                await _BEMT2005Queries.DeleteData(BEMT2003_Delete.APK); // Xóa dữ liệu bảng master dữ liệu đọc từ file đính kèm
                await _BEMT2006Queries.DeleteData(BEMT2003_Delete.APK); // Xóa dữ liệu bảng detail dữ liệu đọc từ file đính kèm
                await _BEMT2004Queries.DeleteData(BEMT2003_Delete.APK); // Xóa dữ liệu bảng chi tiết  kết quả đối chiếu từ AI
                await _BEMT2003Queries.DeleteData(BEMF2000ViewModel.APK); // Xóa dữ liệu bảng chính kết quả đối chiếu từ AI
            }
            catch (Exception)
            {
                return;
            }
            return;
        }

    }
}