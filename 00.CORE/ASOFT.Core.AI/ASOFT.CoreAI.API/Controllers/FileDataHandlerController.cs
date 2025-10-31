using ASOFT.Core.API.Versions;
using ASOFT.CoreAI.Business;
using ASOFT.CoreAI.Business.ChatHandler.FileStorage;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using ASOFT.OO.API.Controllers;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using static ASOFT.CoreAI.Common.AIConstants;
using static ASOFT.CoreAI.Common.EnumConstants;
using IOFile = System.IO.File;

namespace ASOFT.CoreAI.API.Controllers
{
    // hàm AgentPromptController sẽ quản lý các prompt của agent
    [ApiVersion(SupportApiVersions.V_2_0_Str)]
    [ApiExplorerSettings(GroupName = "CoreAI")]
    public class FileDataHandlerController : AgentBaseController
    {
        private readonly IRedisHandler _redisHandler;
        private readonly SettingsManager _settingsManager;
        private readonly IST2131Queries _readFileResultQueries;
        private readonly AgentManager _agentManager;
        private readonly IST2130Queries _agentPromptQueries;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IChatHistoryHandler _chatHistoryHandler;
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly HandlerOCRLocalService _handlerOCRLocalService;

        public FileDataHandlerController(IRedisHandler redisHandler, SettingsManager settingsManager,
            IST2131Queries readFileResultQueries, AgentManager agentManager, IST2130Queries agentPromptQueries, IWebHostEnvironment hostEnvironment,
            IChatHistoryHandler chatHistoryHandler, HandlerOCRLocalService handlerOCRLocalService)
        {
            _redisHandler = redisHandler;
            _settingsManager = settingsManager;
            _readFileResultQueries = readFileResultQueries;
            _agentManager = agentManager;
            _agentPromptQueries = agentPromptQueries;
            _hostingEnvironment = hostEnvironment;
            _chatHistoryHandler = chatHistoryHandler;
            _handlerOCRLocalService = handlerOCRLocalService;
        }

        [HttpPost]
        [ActionName("HandlerFile")]
        public async Task<ChatResponseReadFileModel> HandlerFileAsync([FromBody] ReadFileRequest request)
        {
            // 1. Validate request
            if (request == null)
                return ChatHandlerHelper.CreateResponseReadFile("Request body is null.", false);

            if (string.IsNullOrWhiteSpace(request.UserId))
                return ChatHandlerHelper.CreateResponseReadFile("UserId is required.", false);

            if (request.BEMF2002Detail == null)
                return ChatHandlerHelper.CreateResponseReadFile("BEMF2002Detail is required.", false);

            if (request.AttachFiles == null || !request.AttachFiles.Any(x => !string.IsNullOrWhiteSpace(x.AttachURL)))
                return ChatHandlerHelper.CreateResponseReadFile("Invalid request or file path is empty.", false);

            // 2. Load prompt content
            var prompt = await _agentPromptQueries.QueryPromptsByAgentCode(AgentKeys.BEM_AGENT_BEMF2000);
            if (prompt == null || string.IsNullOrWhiteSpace(prompt.PromptContent))
                return ChatHandlerHelper.CreateResponseReadFile("Không tồn tại Prompt!", false);

            try
            {
                var webRootPath = _hostingEnvironment.WebRootPath;

                // Chuẩn hóa đường dẫn file đính kèm
                request.AttachFiles.ForEach(file =>
                {
                    if (string.IsNullOrWhiteSpace(file.AttachURL))
                        return;
                    var relativePath = file.AttachURL
                        .Replace("~\\", string.Empty)
                        .Replace("~", string.Empty)
                        .TrimStart('\\', '/');

                    file.AttachURL = Path.Combine(webRootPath, relativePath).Replace("/", "\\");
                });

                // kiểm tra cấu hình sử dụng OCR local hay dịch vụ OCR
                bool IsHandeleOCRLocal = _settingsManager.GetIsUseServiceReadOCR();

                // 3. OCR
                string ocrTextContent = string.Empty;
                List<ResultReadFileModel> ocrResults = new List<ResultReadFileModel>();
                if (IsHandeleOCRLocal)
                {
                    var filePaths = request.AttachFiles.Where(x => !string.IsNullOrEmpty(x.AttachURL)).Select(x => x.AttachURL).ToList();
                    if (filePaths == null || filePaths.Count == 0)
                        return ChatHandlerHelper.CreateResponseReadFile("No valid file paths for OCR.", false);
                    ocrTextContent = _handlerOCRLocalService.ReadFileOCRWithLocal(filePaths).Result;
                }
                else
                {
                    ocrResults = await _agentManager.ReadAttacheFileOCR(request.AttachFiles);
                    if (ocrResults == null || !ocrResults.Any(x => !string.IsNullOrWhiteSpace(x.TextContent)))
                        return ChatHandlerHelper.CreateResponseReadFile("No text extracted from the file.", false);

                    // 4. Tổng hợp nội dung OCR
                    var sb = new StringBuilder();
                    int i = 0;
                    foreach (var item in ocrResults)
                    {
                        sb.AppendLine($"📄 File {++i}: **{item.FileName}**");
                        sb.AppendLine(item.TextContent);
                        sb.AppendLine();
                    }
                    ocrTextContent = sb.ToString();
                }
                // 5. Load training data
                var indexName = AgentKeyHelper.GetIndexKey(AgentKeys.BEM_AGENT_BEMF2000);
                var maxRecords = _settingsManager.GetNumberRecords().maxTraining;
                var trainingData = await _redisHandler.GetDataByReadFileAsync(request, indexName, maxRecords);
                var resultReadFile = new ST2131
                {
                    APK = Guid.NewGuid(),
                    APKMaster = request.BEMF2002Detail.APK,
                    AttachName = "Thông tin kết quả đối chiếu",
                    CreateUserID = request.UserId,
                    CreateDate = DateTime.Now,
                    TextContentOCR = ocrTextContent,
                    DivisionID = request.BEMF2002Detail.DivisionID,
                    AttachID = request.AttachFiles.Select(x => x.AttachID).FirstOrDefault(),
                };

                if (!string.IsNullOrWhiteSpace(ocrTextContent))
                {
                    request.Question = "Hãy đối chiếu dữ liệu đọc được từ OCR với dữ liệu ở người dùng cung cấp (datas) cho tôi";
                    string result = string.Empty;
                    if (IsHandeleOCRLocal)
                    {
                        // Tạo kết quả OCR từ nội dung text
                        result = await _agentManager.SendPromptWithLocalsAsync(
                                              request,
                                              prompt.PromptContent,
                                              ocrTextContent,
                                              Enumerable.Empty<ChatHistoryResponseModel>(),
                                              trainingData,
                                              new List<BEMF2002DetailModel> { request.BEMF2002Detail },
                                              request.BEMT2001Models ?? new List<BEMT2001Model>()
                                          );
                    }
                    else
                    {
                        result = await _agentManager.SendPromptWithReadFile(
                                               request,
                                               prompt.PromptContent,
                                               ocrResults,
                                               Enumerable.Empty<ChatHistoryResponseModel>(),
                                               trainingData,
                                               new List<BEMF2002DetailModel> { request.BEMF2002Detail },
                                               request.BEMT2001Models ?? new List<BEMT2001Model>()
                                           );
                    }

                    resultReadFile.TextContentAI = !string.IsNullOrWhiteSpace(result) ? result : "Không có kết quả đối chiếu";
                    var matchResult = ExtractMatchInfo(result);

                    if (!string.IsNullOrEmpty(matchResult.MatchRate))
                        resultReadFile.Percentage = matchResult.MatchRate;

                    if (!string.IsNullOrEmpty(matchResult.Conclusion))
                        resultReadFile.Status = matchResult.Conclusion;
                }
                else
                {
                    resultReadFile.TextContentOCR = "Hiện tại chưa có dữ liệu OCR để xử lý.";
                    resultReadFile.TextContentAI = "Không có kết quả đối chiếu";
                    resultReadFile.Status = StatusCompareOCR.UNDEFINED.ToString();
                }

                bool isSave = await _readFileResultQueries.CreateFileResult(resultReadFile);
                return ChatHandlerHelper.CreateResponseReadFile(
                    isSave ? "Đọc và ghi kết quả thành công" : "Đọc và ghi kết quả không thành công", isSave);
            }
            catch (Exception ex)
            {
                return ChatHandlerHelper.CreateResponseReadFile($"Error processing file: {ex.Message}", false);
            }
        }

        private OcrMatchResult ExtractMatchInfo(string input)
        {
            var result = new OcrMatchResult();

            // Tỷ lệ hợp lệ: - **Tỷ lệ hợp lệ:** 87.5%
            string matchRate = string.Empty;
            var keywordRate = "Tỷ lệ hợp lệ";
            var indexRate = input.IndexOf(keywordRate, StringComparison.OrdinalIgnoreCase);

            if (indexRate >= 0)
            {
                var remaining = input.Substring(indexRate);
                var lines = remaining.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    // Dòng chứa % là dòng kết quả
                    if (line.Contains("%"))
                    {
                        // Tìm phần có số %, ví dụ: "87.5%"
                        var percentMatch = Regex.Match(line, @"([\d.,]+)%");
                        if (percentMatch.Success)
                        {
                            matchRate = percentMatch.Groups[1].Value + "%";
                            break;
                        }
                    }
                }
                result.MatchRate = matchRate;
            }
            // Kết luận tổng thể: - **Kết luận tổng thể:**\n  - ✅ OK – Hầu hết các tiêu chí đều khớp
            string conclusion = string.Empty;

            var keyword = "Kết luận tổng thể";
            var index = input.IndexOf(keyword, StringComparison.OrdinalIgnoreCase);

            if (index >= 0)
            {
                // Cắt phần còn lại sau "Kết luận tổng thể"
                var remaining = input.Substring(index);

                // Tìm dòng có dấu kết luận
                var lines = remaining.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    if (line.Contains("OK"))
                    {
                        conclusion = "✅OK";
                        break;
                    }
                    if (line.Contains("NG"))
                    {
                        conclusion = "❌NG";
                        break;
                    }
                    if (line.Contains("BLANK"))
                    {
                        conclusion = "⚠️BLANK";
                        break;
                    }
                }
                result.Conclusion = conclusion;
            }
            return result;
        }

        [HttpPost]
        [ActionName("UploadFile")]
        public async Task<ChatResponseModel> UploadFileAsync([FromForm] List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return ChatHandlerHelper.CreateResponse(Guid.Empty, "No file uploaded");

            var webRootPath = _hostingEnvironment.WebRootPath;
            var folderPath = Path.Combine(webRootPath, "Attached", "AI");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fullPathList = new List<string>();

            foreach (var file in files.Where(f => f.Length > 0))
            {
                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var ext = Path.GetExtension(file.FileName);
                var uniqueFileName = $"{fileName}_{Guid.NewGuid():N}{ext}";
                var fullPath = Path.Combine(folderPath, uniqueFileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                fullPathList.Add(fullPath);
            }

            string resultString = string.Join(",", fullPathList);
            return ChatHandlerHelper.CreateResponse(Guid.Empty, resultString);
        }

        [HttpPost]
        [ActionName("CreateFile")]
        public async Task<ChatResponseReadFileModel> CreateFileAsync(ReadFileRequest request)
        {
            if (request == null)
                return ChatHandlerHelper.CreateResponseReadFile("Request body is null.", false);

            if (string.IsNullOrWhiteSpace(request.TextContent))
                return ChatHandlerHelper.CreateResponseReadFile("TextContent body is null.", false);

            request.Question = "Hãy đối chiếu dữ liệu đọc được từ OCR với dữ liệu ở người dùng cung cấp (datas) cho tôi";

            var prompt = await _agentPromptQueries.QueryPromptsByAgentCode(AgentKeys.BEM_AGENT_BEMF2000_CREATEFILE);
            if (prompt == null || string.IsNullOrWhiteSpace(prompt.PromptContent))
                return ChatHandlerHelper.CreateResponseReadFile("Không tồn tại Prompt!", false);

            var indexName = AgentKeyHelper.GetIndexKey(AgentKeys.BEM_AGENT_BEMF2000_CREATEFILE);
            var maxRecords = _settingsManager.GetNumberRecords().maxTraining;
            var trainingData = await _redisHandler.GetDataByReadFileAsync(request, indexName, maxRecords);

            var result = await _agentManager.SendPromptWithReadFile(
                request,
                prompt.PromptContent,
                new List<ResultReadFileModel>(),
                Enumerable.Empty<ChatHistoryResponseModel>(),
                trainingData,
                new List<BEMF2002DetailModel>(),
                new List<BEMT2001Model>(),
                request.TextContent
            );

            if (string.IsNullOrWhiteSpace(result))
                return ChatHandlerHelper.CreateResponseReadFile("Không có kết quả tạo file", false);

            string url = await ExportExcelFromAIAsync(result);
            return ChatHandlerHelper.CreateResponseReadFile(url, true);
        }

        // hàm lấy base url từ settings
        private async Task<string> GetBaseUrlAsync()
        {
            return await _settingsManager.GetExternalApi();
        }

        // hàm xuất dữ liệu từ AI sang file Excel
        private async Task<string> ExportExcelFromAIAsync(string aiCsvData)
        {
            Guid Id = Guid.NewGuid();
            var fileName = $"KetQuaDoiChieu_{Id}.xlsx";
            var webRootPath = _hostingEnvironment.WebRootPath;
            var filePath = Path.Combine(webRootPath, "downloads", fileName);

            var lines = aiCsvData.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Chi tiết");

            int currentRow = 1;
            bool isFooterStarted = false;

            foreach (var line in lines)
            {
                // Bỏ dòng phân cách bảng như |----| hoặc bảng markdown line giữa header và body
                if (Regex.IsMatch(line, @"^\|[-\s|]+$")) continue;

                // Nếu là bảng tổng hợp cuối (footer)
                if (line.Trim().StartsWith("| Tổng") || line.Trim().StartsWith("|Tổng"))
                {
                    isFooterStarted = true;
                    worksheet = workbook.Worksheets.Add("Tổng hợp");
                    currentRow = 1;
                }

                var columns = Regex.Matches(line, @"\|([^|]+)")
                                   .Cast<Match>()
                                   .Select(m => m.Groups[1].Value.Trim())
                                   .ToList();

                for (int col = 0; col < columns.Count; col++)
                {
                    worksheet.Cell(currentRow, col + 1).Value = columns[col];

                    // Header format
                    if (!isFooterStarted && currentRow == 1)
                    {
                        worksheet.Cell(currentRow, col + 1).Style.Font.Bold = true;
                        worksheet.Cell(currentRow, col + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                    }
                }

                currentRow++;
            }

            workbook.SaveAs(filePath);
            string baseUrl = await GetBaseUrlAsync();
            var downloadUrl = $"http://192.168.0.201:9980/downloads/{fileName}";
            return downloadUrl;
        }

        [HttpPost]
        [ActionName("ReadFileOCR")]
        public async Task<ChatResponseReadFileModel> ReadFileOCR(ReadFileRequest request)
        {
            if (request == null)
            {
                return ChatHandlerHelper.CreateResponseReadFile("Không có dữ liệu", true);
            }
            if (request.FilePaths == null || request.FilePaths.Count == 0)
            {
                return ChatHandlerHelper.CreateResponseReadFile("Không có thông tin về file cần đọc", true);
            }
            var url = "http://192.168.2.125:4444/ocr";

            using var form = new MultipartFormDataContent();

            var provider = new FileExtensionContentTypeProvider();

            foreach (var filePath in request.FilePaths)
            {
                if (!System.IO.File.Exists(filePath))
                    throw new FileNotFoundException($"Không tìm thấy file: {filePath}");

                var fileStream = System.IO.File.OpenRead(filePath);
                var fileContent = new StreamContent(fileStream);

                // Lấy content-type dựa theo phần mở rộng
                if (!provider.TryGetContentType(filePath, out var contentType))
                    contentType = "application/octet-stream"; // fallback

                fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                form.Add(fileContent, "files", Path.GetFileName(filePath));
            }
            using var response = await _httpClient.PostAsync(url, form);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            string outputPath = @"C:\Users\ducmanh\Desktop\ResultOCR.txt";
            if (!string.IsNullOrEmpty(outputPath))
            {
                await IOFile.WriteAllTextAsync(outputPath, result);
            }

            return ChatHandlerHelper.CreateResponseReadFile(result, true);
        }
    }
}