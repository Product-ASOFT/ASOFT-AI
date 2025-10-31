using ASOFT.Core.API.Versions;
using ASOFT.CoreAI.Business;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using ASOFT.OO.API.Controllers;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using static ASOFT.CoreAI.Common.AIConstants;
using IOFile = System.IO.File;

namespace ASOFT.CoreAI.API.Controllers
{
    // hàm AgentPromptController sẽ quản lý các prompt của agent
    [ApiVersion(SupportApiVersions.V_2_0_Str)]
    [ApiExplorerSettings(GroupName = "CoreAI")]
    public class FileDataHandlerController : AgentBaseController
    {
        private readonly IRedisHandler _redisHandler;
        private readonly SettingsManagerService _settingsManager;
        private readonly AgentManagerService _agentManager;
        private readonly IST2130Queries _agentPromptQueries;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly ReadFileOrchestratorService _orchestrator;

        public FileDataHandlerController(IRedisHandler redisHandler, SettingsManagerService settingsManager,
            IST2131Queries readFileResultQueries, AgentManagerService agentManager, IST2130Queries agentPromptQueries, IWebHostEnvironment hostEnvironment,
            IChatHistoryHandler chatHistoryHandler, ReadFileOrchestratorService orchestrator)
        {
            _redisHandler = redisHandler;
            _settingsManager = settingsManager;
            _agentManager = agentManager;
            _agentPromptQueries = agentPromptQueries;
            _hostingEnvironment = hostEnvironment;
            _orchestrator = orchestrator;
        }

        [HttpPost]
        [ActionName("HandlerFile")]
        public async Task<ChatResponseReadFileModel> HandlerFileAsync([FromBody] ReadFileRequest request)
        {
            var res = await _orchestrator.HandleAsync(request);
            return res;
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