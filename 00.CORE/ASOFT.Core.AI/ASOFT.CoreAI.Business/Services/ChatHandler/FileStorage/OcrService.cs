using ASOFT.CoreAI.Abstractions;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using ASOFT.CoreAI.Infrastructure.Interface;
using ClosedXML.Excel;
using Google.Cloud.Vision.V1;
using HeyRed.Mime;
using Microsoft.AspNetCore.StaticFiles;
using PdfiumViewer;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http.Headers;
using System.Runtime.Versioning;
using System.Text;
using UglyToad.PdfPig;
using Xceed.Words.NET;
using PdfDocument = PdfiumViewer.PdfDocument;

namespace ASOFT.CoreAI.Business.Services.ChatHandler.FileStorage
{
    public sealed class OcrService : IOCRService
    {
        private readonly IRedisMemoryProvider _redis;
        private readonly SettingsManagerService _settings;
        private readonly HttpClient _httpClient;

        private static readonly string[] ImageMimeTypes = new[]
        {
            "image/jpeg", "image/png", "image/gif", "image/bmp", "image/tiff"
        };

        public OcrService(
            IRedisMemoryProvider redisMemoryProvider,
            SettingsManagerService settingsManager,
            IHttpClientFactory httpClientFactory)
        {
            _redis = redisMemoryProvider;
            _settings = settingsManager;
            _httpClient = httpClientFactory.CreateClient(nameof(OcrService));
        }

        /// <summary>
        /// Đọc text từ danh sách file (PDF, Word, Excel, Image).
        /// Trả về text gộp + list kết quả từng file.
        /// </summary>
        public async Task<(string TextMerged, List<ResultReadFileModel> Results)> ReadAsync(IReadOnlyList<AttachFileModel> files, Guid APK)
        {
            if (files == null)
            {
                return (string.Empty, new List<ResultReadFileModel>());
            }

            if (files.Count == 0)
            {
                return (string.Empty, new List<ResultReadFileModel>());
            }

            var results = await ExtractFilesAsync(files, APK).ConfigureAwait(false);
            if (results == null || results.Count() == 0)
            {
                return (string.Empty, new List<ResultReadFileModel>());
            }

            if (!results.Any(r => r != null && !string.IsNullOrWhiteSpace(r.TextContent)))
            {
                return (string.Empty, results);
            }

            var sb = new StringBuilder();
            for (int i = 0; i < results.Count; i++)
            {
                var r = results[i];
                if (r == null)
                {
                    continue;
                }

                var text = r.TextContent;
                if (string.IsNullOrWhiteSpace(text))
                {
                    continue;
                }

                sb.AppendLine($"📄 File {i + 1}: **{r.FileName}**");
                sb.AppendLine(text);
                sb.AppendLine();
            }

            var merged = sb.ToString();
            return (merged, results);
        }

        // ========================= Core Extraction =========================

        private async Task<List<ResultReadFileModel>> ExtractFilesAsync(IReadOnlyList<AttachFileModel> attachFiles, Guid APK)
        {
            var bag = new ConcurrentBag<ResultReadFileModel>();
            var isUseLocal = _settings.GetIsUseServiceReadOCR();
            int numberOrder = 0;
            await Parallel.ForEachAsync(attachFiles, async (attach, ct) =>
            {
                var result = new ResultReadFileModel();
                result.NumberOrder = Interlocked.Increment(ref numberOrder);
                if (attach != null)
                {
                    result.FilePath = attach.AttachURL ?? string.Empty;
                    result.AttachID = attach.AttachID;
                    result.FileName = Path.GetFileName(result.FilePath);
                }
                if (string.IsNullOrWhiteSpace(result.FilePath))
                {
                    bag.Add(result);
                    return;
                }
                if (!File.Exists(result.FilePath))
                {
                    bag.Add(result);
                    return;
                }

                var mimeType = MimeTypesMap.GetMimeType(result.FilePath);
                if (string.IsNullOrEmpty(mimeType))
                {
                    bag.Add(result);
                    return;
                }

                var fileInfo = new FileInfo(result.FilePath);
                var cacheKey = string.Format(
                    "FileCache_:{1}:{2}:{3}",
                    APK.ToString(),
                    fileInfo.FullName.ToLowerInvariant(),
                    fileInfo.LastWriteTimeUtc.Ticks,
                    fileInfo.Length
                );

                var cached = await _redis.GetFileCacheAsync(result.FilePath, cacheKey).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(cached))
                {
                    result.TextContent = cached;
                    bag.Add(result);
                    return;
                }

                try
                {
                    result.TextContent = await ExtractByMimeTypeAsync(result.FilePath, mimeType, isUseLocal).ConfigureAwait(false);
                    if (!string.IsNullOrWhiteSpace(result.TextContent))
                    {
                        await _redis.SaveFileCacheAsync(result.FilePath, result.TextContent, cacheKey).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    // Giữ lỗi vào TextContent để trace dễ hơn (tuỳ chọn)
                    result.TextContent = "[ERROR] " + ex.Message;
                }
                bag.Add(result);
            });
            var resultReadFileModels = bag.Where(x => x.HasErrorReadFile == false).ToList();
            if (resultReadFileModels != null && resultReadFileModels.Count() > 0)
            {
                resultReadFileModels.Sort((a, b) => a.NumberOrder.CompareTo(b.NumberOrder));
            }
            return resultReadFileModels;
        }
        private async Task<string> ExtractByMimeTypeAsync(string filePath, string mimeType, bool useLocal)
        {
            if (ImageMimeTypes.Contains(mimeType))
            {
                return await ExtractTextFromImageAsync(filePath, useLocal).ConfigureAwait(false);
            }

            if (mimeType == "application/pdf")
            {
                return await HandlePdfAsync(filePath, useLocal).ConfigureAwait(false);
            }

            if (mimeType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                || mimeType == "application/msword")
            {
                return await ExtractTextFromWordAsync(filePath).ConfigureAwait(false);
            }

            if (mimeType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                || mimeType == "application/vnd.ms-excel")
            {
                return await ExtractTextFromExcelAsync(filePath).ConfigureAwait(false);
            }

            return string.Empty;
        }

        // ========================= PDF =========================
        [SupportedOSPlatform("windows6.1")]
        private async Task<string> HandlePdfAsync(string path, bool useLocal)
        {
            var isTextPdf = await IsTextPdfWithPdfiumAsync(path).ConfigureAwait(false);
            if (isTextPdf)
            {
                return await ReadTextFromPdfAsync(path).ConfigureAwait(false);
            }
            else
            {
                return await ExtractTextFromPdfImagesAsync(path, useLocal).ConfigureAwait(false);
            }
        }

        private static async Task<bool> IsTextPdfWithPdfiumAsync(string filePath)
        {
            await using (var fs = File.OpenRead(filePath))
            {
                using (var doc = PdfDocument.Load(fs))
                {
                    for (int i = 0; i < doc.PageCount; i++)
                    {
                        var text = doc.GetPdfText(i);
                        if (text != null)
                        {
                            text = text.Trim();
                            if (!string.IsNullOrWhiteSpace(text))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        [SupportedOSPlatform("windows6.1")]
        private async Task<string> ExtractTextFromPdfImagesAsync(string pdfPath, bool useLocal)
        {
            var texts = new List<string>();
            if (useLocal)
            {
                var list = new List<string> { pdfPath };
                string content = await ReadFileOCRWithLocalAsync(list).ConfigureAwait(false);
                texts.Add(content);
            }
            else
            {
                var pages = ConvertPdfToImages(pdfPath);
                foreach (var img in pages)
                {
                    await using (var ms = new MemoryStream())
                    {
                        img.Save(ms, ImageFormat.Png);
                        ms.Position = 0;
                        var visionImage = Google.Cloud.Vision.V1.Image.FromStream(ms);
                        string content = await ReadImageOcrWithGoogleAsync(visionImage).ConfigureAwait(false);

                        if (content == null)
                        {
                            content = string.Empty;
                        }
                        texts.Add(content);
                    }
                    img.Dispose();
                }
            }
            return string.Join("\n---PAGE---\n", texts);
        }


        [SupportedOSPlatform("windows6.1")]
        private static List<Bitmap> ConvertPdfToImages(string pdfFilePath)
        {
            var images = new List<Bitmap>();
            using (var doc = PdfiumViewer.PdfDocument.Load(pdfFilePath))
            {
                for (int i = 0; i < doc.PageCount; i++)
                {
                    using (var rendered = doc.Render(i, 300, 300, true))
                    {
                        var bmp = new Bitmap(rendered);
                        images.Add(bmp);
                    }
                }
            }
            return images;
        }

        private static async Task<string> ReadTextFromPdfAsync(string path)
        {
            var sb = new StringBuilder();
            using (var doc = UglyToad.PdfPig.PdfDocument.Open(path))
            {
                foreach (var page in doc.GetPages())
                {
                    if (page != null && !string.IsNullOrWhiteSpace(page.Text))
                    {
                        sb.AppendLine(page.Text.Trim());
                        sb.AppendLine("\n---PAGE BREAK---\n");
                    }
                }
            }
            return await Task.FromResult(sb.ToString());
        }

        // ========================= Image =========================

        private async Task<string> ExtractTextFromImageAsync(string path, bool useLocal)
        {
            if (!File.Exists(path))
            {
                return string.Empty;
            }

            if (useLocal)
            {
                var list = new List<string> { path };
                var textLocal = await ReadFileOCRWithLocalAsync(list).ConfigureAwait(false);
                return textLocal;
            }
            else
            {
                var image = Google.Cloud.Vision.V1.Image.FromFile(path);
                var textCloud = await ReadImageOcrWithGoogleAsync(image).ConfigureAwait(false);
                return textCloud;
            }
        }

        private async Task<string> ReadImageOcrWithGoogleAsync(Google.Cloud.Vision.V1.Image image)
        {
            var client = await ImageAnnotatorClient.CreateAsync().ConfigureAwait(false);
            var res = await client.DetectDocumentTextAsync(image).ConfigureAwait(false);
            if (res == null)
            {
                return string.Empty;
            }

            if (string.IsNullOrWhiteSpace(res.Text))
            {
                return string.Empty;
            }

            return res.Text;
        }

        // ========================= Word / Excel =========================

        private static async Task<string> ExtractTextFromWordAsync(string path)
        {
            using (var doc = DocX.Load(path))
            {
                var text = doc.Text;
                if (text == null)
                {
                    return string.Empty;
                }

                return await Task.FromResult(text);
            }
        }

        private static async Task<string> ExtractTextFromExcelAsync(string path)
        {
            using (var wb = new XLWorkbook(path))
            {
                var sb = new StringBuilder();

                foreach (var ws in wb.Worksheets)
                {
                    sb.AppendLine("--- Sheet: " + ws.Name + " ---");

                    foreach (var row in ws.RowsUsed())
                    {
                        foreach (var cell in row.CellsUsed())
                        {
                            var value = cell.GetValue<string>();
                            if (value != null)
                            {
                                sb.Append(value);
                            }
                            sb.Append('\t');
                        }
                        sb.AppendLine();
                    }

                    sb.AppendLine();
                }

                var text = sb.ToString();
                return await Task.FromResult(text);
            }
        }

        // ========================= OCR Local =========================

        private async Task<string> ReadFileOCRWithLocalAsync(List<string> filePaths)
        {
            var ocrUrl = _settings.GetUrlReadOCR();
            if (string.IsNullOrWhiteSpace(ocrUrl))
            {
                throw new InvalidOperationException("Chưa cấu hình URL OCR.");
            }

            using (var form = new MultipartFormDataContent())
            {
                var provider = new FileExtensionContentTypeProvider();

                foreach (var filePath in filePaths)
                {
                    if (!File.Exists(filePath))
                    {
                        continue;
                    }

                    var stream = File.OpenRead(filePath);
                    var content = new StreamContent(stream);

                    string contentType;
                    var responeSuccess = provider.TryGetContentType(filePath, out contentType);
                    if (!responeSuccess || string.IsNullOrWhiteSpace(contentType))
                    {
                        contentType = "application/octet-stream";
                    }

                    content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                    form.Add(content, "files", Path.GetFileName(filePath));
                }

                var response = await _httpClient.PostAsync(ocrUrl, form).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (result == null)
                {
                    return string.Empty;
                }

                return result;
            }
        }
    }
}
