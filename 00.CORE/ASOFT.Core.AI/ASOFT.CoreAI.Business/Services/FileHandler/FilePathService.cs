using ASOFT.CoreAI.Business.Services.RedisHandler;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static ASOFT.CoreAI.Common.AIConstants;

namespace ASOFT.CoreAI.Business
{
    public class FilePathService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IST2130Queries _ST2130Queries;
        private readonly SettingsManagerService _settings;
        private readonly AgentPromptService _agentPromptService;
        private readonly IRedisService _redisService;
        public FilePathService(IWebHostEnvironment hostingEnvironment,
            IST2130Queries ST2130Queries,
            SettingsManagerService settings,
            AgentPromptService agentPromptService,
            IRedisService redisService)
        {
            _hostingEnvironment = hostingEnvironment;
            _ST2130Queries = ST2130Queries;
            _settings = settings;
            _agentPromptService = agentPromptService;
            _redisService = redisService;
        }
        public IReadOnlyList<AttachFileModel> NormalizeToPhysicalUnderWebRoot(IEnumerable<AttachFileModel> files)
        {
            var webRootPath = _hostingEnvironment.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRootPath)) return Array.Empty<AttachFileModel>();
            if (files == null) return Array.Empty<AttachFileModel>();

            var list = new List<AttachFileModel>();
            foreach (var f in files)
            {
                if (string.IsNullOrWhiteSpace(f?.AttachURL)) continue;

                var relative = f.AttachURL
                    .Replace("~\\", string.Empty)
                    .Replace("~", string.Empty)
                    .TrimStart('\\', '/')
                    .Replace("/", "\\");
                var abs = Path.GetFullPath(Path.Combine(webRootPath, relative));
                if (File.Exists(abs))
                {
                    list.Add(new AttachFileModel
                    {
                        AttachID = f.AttachID,
                        AttachName = f.AttachName,
                        AttachURL = abs
                    });
                }
            }
            return list;
        }

        public async Task<ChatResponseModel> UpLoadFile(List<IFormFile> files)
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
        // hàm lấy base url từ settings
        private async Task<string> GetBaseUrlAsync()
        {
            return await _settings.GetExternalApi();
        }

        public async Task<ChatResponseReadFileModel> CreateFile(ReadFileRequest request)
        {
            if (request == null)
                return ChatHandlerHelper.CreateResponseReadFile("Request body is null.", false);

            if (string.IsNullOrWhiteSpace(request.TextContent))
                return ChatHandlerHelper.CreateResponseReadFile("TextContent body is null.", false);

            request.Question = "Hãy đối chiếu dữ liệu đọc được từ OCR với dữ liệu ở người dùng cung cấp (datas) cho tôi";

            var prompt = await _ST2130Queries.QueryPromptsByAgentCode(AgentKeys.BEM_AGENT_BEMF2000_CREATEFILE);
            if (prompt == null || string.IsNullOrWhiteSpace(prompt.PromptContent))
                return ChatHandlerHelper.CreateResponseReadFile("Không tồn tại Prompt!", false);

            var indexName = AgentKeyHelper.GetIndexKey(AgentKeys.BEM_AGENT_BEMF2000_CREATEFILE);
            var maxRecords = _settings.GetNumberRecords().maxTraining;
            var trainingData = await _redisService.GetDataByReadFileAsync(request, indexName, maxRecords);

            var result = await _agentPromptService.SendPromptWithReadFile(
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
    }
}
