using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;
using static ASOFT.CoreAI.Common.AIConstants;
using iText.Kernel.Pdf;

namespace ASOFT.CoreAI.Business
{
    public class FilePathService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IST2130Queries _ST2130Queries;
        private readonly SettingsManagerService _settings;
        private readonly AgentPromptService _agentPromptService;
        private readonly IRedisService _redisService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FilePathService(IWebHostEnvironment hostingEnvironment,
            IST2130Queries ST2130Queries,
            SettingsManagerService settings,
            AgentPromptService agentPromptService,
            IRedisService redisService,
            IHttpContextAccessor httpContextAccessor)
        {
            _hostingEnvironment = hostingEnvironment;
            _ST2130Queries = ST2130Queries;
            _settings = settings;
            _agentPromptService = agentPromptService;
            _redisService = redisService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ChatResponseModel> UpLoadFile(List<IFormFile> files, bool IsCompare)
        {
            if (files == null || files.Count == 0)
                return ChatHandlerHelper.CreateResponse(Guid.Empty, "No file uploaded", false);

            var webRootPath = _hostingEnvironment.WebRootPath;
            var folderPath = Path.Combine(webRootPath, "Attached", "AI");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fullPathList = new List<string>();

            foreach (var file in files.Where(f => f.Length > 0))
            {
                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var ext = Path.GetExtension(file.FileName);
                var uniqueFileName = $"{fileName}{ext}";
                if (!IsCompare)
                {
                    uniqueFileName = $"{fileName}_{Guid.NewGuid():N}{ext}";
                }
                var fullPath = Path.Combine(folderPath, uniqueFileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                fullPathList.Add(fullPath);
            }

            string resultString = string.Join(",", fullPathList);
            return ChatHandlerHelper.CreateResponse(Guid.Empty, resultString, true);
        }

        public async Task<ChatResponseReadFileModel> CreateFile(ReadFileRequest request)
        {
            if (request == null)
                return ChatHandlerHelper.CreateResponseReadFile("Request body is null.", false);

            if (string.IsNullOrWhiteSpace(request.TextContent))
                return ChatHandlerHelper.CreateResponseReadFile("TextContent body is null.", false);

            var prompt = await _ST2130Queries.GetPromptByCode(AgentKeys.BEM_AGENT_BEMF2000_CREATEFILE);
            if (prompt == null || string.IsNullOrWhiteSpace(prompt.PromptContent))
                return ChatHandlerHelper.CreateResponseReadFile("Không tồn tại Prompt!", false);

            var indexName = AgentKeyHelper.GetIndexKey(AgentKeys.BEM_AGENT_BEMF2000_CREATEFILE);
            var maxRecords = await _settings.GetNumberRecordsAsync();
            var trainingData = await _redisService.GetDataByReadFileAsync(request, indexName, maxRecords.maxTraining);

            var result = await _agentPromptService.SendPromptWithReadFile(
                request,
                prompt.Description,
                prompt.PromptContent,
                new List<ResultReadFileModel>(),
                Enumerable.Empty<ChatHistoryResponseModel>(),
                trainingData,
                new List<BEMF2000ViewModel>(),
                new List<BEMF2001ViewModel>(),
                request.TextContent
            );

            if (string.IsNullOrWhiteSpace(result))
                return ChatHandlerHelper.CreateResponseReadFile("Không có kết quả tạo file", false);

            string url = await ExportExcelFromAIAsync(result);
            return ChatHandlerHelper.CreateResponseReadFile(url, true);
        }

        private Task<string> ExportExcelFromAIAsync(string aiCsvData)
        {
            var fileName = $"KetQuaDoiChieu_{Guid.NewGuid():N}.xlsx";
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
            var req = _httpContextAccessor.HttpContext!.Request;
            var baseUrl = $"{req.Scheme}://{req.Host}{req.PathBase}";
            return Task.FromResult($"{baseUrl}/downloads/{fileName}");
        }
        public  List<AttachFileModel> SplitEveryNPages_KeepOriginal(AttachFileModel attachFileModel, int pagesPerFile = 3)
        {
            string sourcePdfPath = attachFileModel.AttachURL!;
            if (string.IsNullOrWhiteSpace(sourcePdfPath))
                throw new ArgumentException("sourcePdfPath is null/empty.", nameof(sourcePdfPath));

            if (!File.Exists(sourcePdfPath))
                throw new FileNotFoundException("Source file not found.", sourcePdfPath);

            string extension = Path.GetExtension(sourcePdfPath);
            if (!string.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("File is not a PDF.");

            if (pagesPerFile <= 0)
                throw new ArgumentOutOfRangeException(nameof(pagesPerFile), "pagesPerFile must be > 0.");

            var outputPaths = new List<AttachFileModel>();

            var sourceDir = Path.GetDirectoryName(sourcePdfPath)!;
            var baseName = Path.GetFileNameWithoutExtension(sourcePdfPath);
            var extentsion = Path.GetExtension(sourcePdfPath);
            var fileNameOrigin = Path.GetFileName(sourcePdfPath);
            using var sourcePdf = new PdfDocument(new PdfReader(sourcePdfPath));
            int totalPages = sourcePdf.GetNumberOfPages();

            int part = 1;
            for (int start = 1; start <= totalPages; start += pagesPerFile)
            {
                int end = Math.Min(start + pagesPerFile - 1, totalPages);

                string outFileName = $"{baseName}_part{part:000}" + extentsion;
                string outPath = Path.Combine(sourceDir, outFileName);

                outPath = EnsureUniquePath(outPath);

                using (var newPdf = new PdfDocument(new PdfWriter(outPath)))
                {
                    sourcePdf.CopyPagesTo(start, end, newPdf);
                }

                outputPaths.Add(new AttachFileModel
                {
                    AttachNameOrigin = fileNameOrigin,
                    AttachName = outFileName,
                    AttachURL = outPath,
                    APK = attachFileModel.APK,
                    AttachID = attachFileModel.AttachID
                });
                part++;
            }
            return outputPaths;
        }
        private  string EnsureUniquePath(string path)
        {
            if (!File.Exists(path)) return path;

            string dir = Path.GetDirectoryName(path)!;
            string name = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);

            int i = 1;
            while (true)
            {
                string candidate = Path.Combine(dir, $"{name}({i}){ext}");
                if (!File.Exists(candidate)) return candidate;
                i++;
            }
        }
    }
}