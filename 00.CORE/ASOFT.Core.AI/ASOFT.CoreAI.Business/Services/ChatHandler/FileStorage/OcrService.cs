using ASOFT.CoreAI.Abstractions;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using ClosedXML.Excel;
using Google.Cloud.Vision.V1;
using HeyRed.Mime;
using Microsoft.AspNetCore.StaticFiles;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http.Headers;
using System.Runtime.Versioning;
using System.Text;
using Xceed.Words.NET;
using static ASOFT.CoreAI.Common.AIConstants;
using PdfDocument = PdfiumViewer.PdfDocument;

namespace ASOFT.CoreAI.Business
{
    public sealed class OcrService : IOCRService
    {
        private readonly IRedisMemoryProvider _redis;
        private readonly SettingsManagerService _settings;
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _clientFactory;

        public OcrService(
            IRedisMemoryProvider redisProvider,
            SettingsManagerService settingsManager,
            IHttpClientFactory httpClientFactory,
            IHttpClientFactory clientFactory)
        {
            _redis = redisProvider;
            _settings = settingsManager;
            _httpClient = httpClientFactory.CreateClient(nameof(OcrService));
            _clientFactory = clientFactory;
        }

        #region ==== PUBLIC METHOD ====

        /// <summary>
        /// Đọc text từ danh sách file (PDF, Word, Excel, Image)
        /// </summary>
        [SupportedOSPlatform("windows6.1")]
        public async Task<(string TextMerged, List<ResultReadFileModel> Results)> ReadAsync(IReadOnlyList<AttachFileModel> files, Guid APK)
        {
            if (files == null || files.Count == 0)
                return (string.Empty, new List<ResultReadFileModel>());

            var results = await ExtractFilesAsync(files, APK);
            if (results == null || results.Count == 0)
                return (string.Empty, new List<ResultReadFileModel>());

            // Gộp text các file
            var sb = new StringBuilder();
            foreach (var (r, index) in results.Select((x, i) => (x, i + 1)))
            {
                if (string.IsNullOrWhiteSpace(r?.TextContent))
                    continue;

                sb.AppendLine($"📄 File {index}: **{r.FileName}**");
                sb.AppendLine(r.TextContent);
                sb.AppendLine();
            }

            return (sb.ToString(), results);
        }

        #endregion ==== PUBLIC METHOD ====

        #region ==== CORE EXTRACTION ====

        [SupportedOSPlatform("windows6.1")]
        private async Task<List<ResultReadFileModel>> ExtractFilesAsync(IReadOnlyList<AttachFileModel> files, Guid apk)
        {
            var results = new ConcurrentBag<ResultReadFileModel>();
            var useLocal = await _settings.GetIsUseServiceReadOCRAsync();
            int order = 0;


            await Parallel.ForEachAsync( files,new ParallelOptions { MaxDegreeOfParallelism = 7 },
                async (attach, ct) =>
                {
                    var result = InitResultModel(attach, Interlocked.Increment(ref order));

                    if (!File.Exists(result.FilePath))
                    {
                        results.Add(result);
                        return;
                    }

                    var mimeType = MimeTypesMap.GetMimeType(result.FilePath);
                    if (string.IsNullOrEmpty(mimeType))
                    {
                        results.Add(result);
                        return;
                    }

                    var fileInfo = new FileInfo(result.FilePath);
                    var cacheKey = $"FileCache_{apk}:{fileInfo.Name.ToLowerInvariant()}:{fileInfo.Length}";

                    var cached = await _redis.GetFileCacheAsync(result.FilePath, cacheKey);
                    if (!string.IsNullOrEmpty(cached))
                    {
                        result.TextContent = cached;
                        results.Add(result);
                        return;
                    }

                    try
                    {
                        result.TextContent = await ExtractByMimeTypeAsync(result.FilePath, mimeType, useLocal);
                        if (!string.IsNullOrWhiteSpace(result.TextContent))
                            await _redis.SaveFileCacheAsync(result.FilePath, result.TextContent, cacheKey);
                    }
                    catch (Exception ex)
                    {
                        result.HasErrorReadFile = true;
                        result.TextContent = "[ERROR] " + ex.Message;
                    }

                    results.Add(result);
                });

            //}

            return results.Where(x => !x.HasErrorReadFile).OrderBy(x => x.NumberOrder).ToList();
        }

        // Khởi tạo model kết quả

        private ResultReadFileModel InitResultModel(AttachFileModel attach, int numberOrder)
        {
            return new ResultReadFileModel
            {
                NumberOrder = numberOrder,
                FilePath = attach?.AttachURL ?? string.Empty,
                AttachID = attach?.AttachID ?? 1,
                FileName = attach?.AttachName ?? string.Empty
            };
        }

        // Phân loại và xử lý theo mime type
        [SupportedOSPlatform("windows6.1")]
        private async Task<string> ExtractByMimeTypeAsync(string filePath, string mimeType, bool useLocal)
        {
            if (MimeTypesConstants.ImageTypes.Contains(mimeType))
                return await ExtractTextFromImageAsync(filePath, useLocal);

            if (mimeType.Equals(MimeTypesConstants.Pdf, StringComparison.OrdinalIgnoreCase))
                return await HandlePdfAsync(filePath, useLocal);

            if (mimeType.Equals(MimeTypesConstants.WordDocx, StringComparison.OrdinalIgnoreCase) ||
                mimeType.Equals(MimeTypesConstants.WordDoc, StringComparison.OrdinalIgnoreCase))
                return await ExtractTextFromWordAsync(filePath);

            if (mimeType.Equals(MimeTypesConstants.ExcelXlsx, StringComparison.OrdinalIgnoreCase))
                return await ExtractTextFromExcelAsync(filePath);

            if (mimeType.Equals(MimeTypesConstants.ExcelXls, StringComparison.OrdinalIgnoreCase))
                return await ReadFileOCRWithLocalAsync(new List<string> { filePath });

            return string.Empty;
        }

        #endregion ==== CORE EXTRACTION ====

        #region ==== PDF HANDLER ====

        [SupportedOSPlatform("windows6.1")]
        private async Task<string> HandlePdfAsync(string path, bool useLocal)
        {
            if (await IsTextPdfAsync(path))
                return await ReadTextFromPdfAsync(path);

            return await ExtractTextFromPdfImagesAsync(path, useLocal);
        }

        private static async Task<bool> IsTextPdfAsync(string filePath)
        {
            await using var fs = File.OpenRead(filePath);
            using var doc = PdfDocument.Load(fs);
            for (int i = 0; i < doc.PageCount; i++)
            {
                var text = doc.GetPdfText(i)?.Trim();
                if (!string.IsNullOrWhiteSpace(text))
                    return true;
            }
            return false;
        }

        [SupportedOSPlatform("windows6.1")]
        private async Task<string> ExtractTextFromPdfImagesAsync(string pdfPath, bool useLocal)
        {
            if (useLocal)
                return await ReadFileOCRWithLocalAsync(new List<string> { pdfPath });

            var texts = new List<string>();
            foreach (var img in ConvertPdfToImages(pdfPath))
            {
                await using var ms = new MemoryStream();
                img.Save(ms, ImageFormat.Png);
                ms.Position = 0;

                var visionImg = Google.Cloud.Vision.V1.Image.FromStream(ms);
                var content = await ReadImageOcrWithGoogleAsync(visionImg) ?? string.Empty;
                texts.Add(content);
                img.Dispose();
            }

            return string.Join("\n---PAGE---\n", texts);
        }

        [SupportedOSPlatform("windows6.1")]
        private static List<Bitmap> ConvertPdfToImages(string pdfFilePath)
        {
            var images = new List<Bitmap>();
            using var doc = PdfDocument.Load(pdfFilePath);
            for (int i = 0; i < doc.PageCount; i++)
                images.Add(new Bitmap(doc.Render(i, 300, 300, true)));
            return images;
        }

        private static async Task<string> ReadTextFromPdfAsync(string path)
        {
            var sb = new StringBuilder();
            using var doc = UglyToad.PdfPig.PdfDocument.Open(path);
            foreach (var page in doc.GetPages())
            {
                if (!string.IsNullOrWhiteSpace(page?.Text))
                {
                    sb.AppendLine(page.Text.Trim());
                    sb.AppendLine("\n---PAGE BREAK---\n");
                }
            }
            return await Task.FromResult(sb.ToString());
        }

        #endregion ==== PDF HANDLER ====

        #region ==== IMAGE HANDLER ====

        private async Task<string> ExtractTextFromImageAsync(string path, bool useLocal)
        {
            if (!File.Exists(path))
                return string.Empty;

            return useLocal
                ? await ReadFileOCRWithLocalAsync(new List<string> { path })
                : await ReadImageOcrWithGoogleAsync(Google.Cloud.Vision.V1.Image.FromFile(path));
        }

        private async Task<string> ReadImageOcrWithGoogleAsync(Google.Cloud.Vision.V1.Image image)
        {
            var client = await ImageAnnotatorClient.CreateAsync();
            var res = await client.DetectDocumentTextAsync(image);
            return string.IsNullOrWhiteSpace(res?.Text) ? string.Empty : res.Text;
        }

        #endregion ==== IMAGE HANDLER ====

        #region ==== WORD / EXCEL HANDLER ====

        private static async Task<string> ExtractTextFromWordAsync(string path)
        {
            using var doc = DocX.Load(path);
            return await Task.FromResult(doc.Text ?? string.Empty);
        }

        private static async Task<string> ExtractTextFromExcelAsync(string path)
        {
            var sb = new StringBuilder();
            using var wb = new XLWorkbook(path);

            foreach (var ws in wb.Worksheets)
            {
                sb.AppendLine($"--- Sheet: {ws.Name} ---");
                foreach (var row in ws.RowsUsed())
                {
                    foreach (var cell in row.CellsUsed())
                        sb.Append(cell.GetValue<string>() + "\t");
                    sb.AppendLine();
                }
                sb.AppendLine();
            }

            return await Task.FromResult(sb.ToString());
        }

        #endregion ==== WORD / EXCEL HANDLER ====

        #region ==== OCR LOCAL SERVICE ====

        private async Task<string> ReadFileOCRWithLocalAsync(List<string> filePaths)
        {
            var ocrUrl = await _settings.GetUrlReadOCRAsync();
            if (string.IsNullOrWhiteSpace(ocrUrl))
                throw new InvalidOperationException("Chưa cấu hình URL OCR.");

            var client = _clientFactory.CreateClient("OCR");
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10));

            using var form = new MultipartFormDataContent();
            var provider = new FileExtensionContentTypeProvider();

            foreach (var filePath in filePaths)
            {
                if (!File.Exists(filePath)) continue;

                var stream = File.OpenRead(filePath);
                var content = new StreamContent(stream)
                {
                    Headers = { ContentType = new MediaTypeHeaderValue(provider.TryGetContentType(filePath, out var type) ? type : "application/octet-stream") }
                };
                form.Add(content, "files", Path.GetFileName(filePath));
            }
            using var req = new HttpRequestMessage(HttpMethod.Post, ocrUrl) { Content = form };

            using var resp = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, cts.Token);

            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadAsStringAsync(cts.Token) ?? string.Empty;

            //var response = await _httpClient.PostAsync(ocrUrl, form);
            //response.EnsureSuccessStatusCode();

            //return await response.Content.ReadAsStringAsync() ?? string.Empty;
        }

        #endregion ==== OCR LOCAL SERVICE ====
    }
}