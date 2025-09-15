using ASOFT.CoreAI.Abstractions;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using ClosedXML.Excel;
using Google.Cloud.Vision.V1;
using HeyRed.Mime;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using UglyToad.PdfPig;
using Xceed.Words.NET;

namespace ASOFT.CoreAI.Business
{
    public class OcrService
    {
        private static readonly string[] ImageMimeTypes = new[]
         {
            "image/jpeg", "image/png", "image/gif", "image/bmp", "image/tiff"
         };

        //private readonly string _credentialPath;
        private readonly IRedisMemoryProvider _redisMemoryProvider;

        public OcrService(IRedisMemoryProvider redisMemoryProvider)
        {
            _redisMemoryProvider = redisMemoryProvider;
            //_credentialPath = credentialJsonPath;

            // Gán biến môi trường để Vision SDK nhận dạng key
            //Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", _credentialPath);
        }

        public Task<bool> IsTextPdfWithPdfium(string filePath)
        {
            using (var doc = PdfiumViewer.PdfDocument.Load(filePath))
            {
                for (int i = 0; i < doc.PageCount; i++)
                {
                    string text = doc.GetPdfText(i)?.Trim();
                    if (!string.IsNullOrWhiteSpace(text))
                        return Task.FromResult(true);
                }
            }
            return Task.FromResult(false);
        }

        #region xử lý đọc hình ảnh trong file pdf ra text

        // hàm xử lý đọc file PDF khi đã được scan hình ảnh
        public async Task<string> ExtractTextFromPdfAsync(string pdfPath)
        {
            var images = ConvertPdfToImages(pdfPath);
            var resultText = new List<string>();
            var client = await ImageAnnotatorClient.CreateAsync();
            foreach (var img in images)
            {
                using (var ms = new MemoryStream())
                {
                    img.Save(ms, ImageFormat.Png);
                    ms.Position = 0;

                    var image = Google.Cloud.Vision.V1.Image.FromStream(ms);
                    string response = await ReadImageOcrWithGoogle(image);
                    resultText.Add(response);
                }

                img.Dispose(); // Giải phóng bộ nhớ
            }

            return string.Join("\n---PAGE---\n", resultText);
        }

        // Hàm chuyển đổi PDF sang danh sách hình ảnh
        private List<Bitmap> ConvertPdfToImages(string pdfFilePath)
        {
            var images = new List<Bitmap>();
            using (var document = PdfiumViewer.PdfDocument.Load(pdfFilePath))
            {
                for (int i = 0; i < document.PageCount; i++)
                {
                    var img = document.Render(i, 300, 300, true); // Render 300 DPI
                    images.Add(new Bitmap(img));
                }
            }
            return images;
        }

        // Hàm chuyển đổi trực tiếp từ hình ảnh sang text
        private async Task<string> ExtractTextFromImageAsync(string imagePath)
        {
            // Kiểm tra file
            if (!File.Exists(imagePath))
                throw new FileNotFoundException("Ảnh không tồn tại", imagePath);

            var image = await Task.Run(() => Google.Cloud.Vision.V1.Image.FromFile(imagePath));

            var response = await ReadImageOcrWithGoogle(image);
            return response ?? string.Empty;
        }

        private async Task<string> ReadImageOcrWithGoogle(Google.Cloud.Vision.V1.Image image)
        {
            var client = await ImageAnnotatorClient.CreateAsync();
            var response = await client.DetectDocumentTextAsync(image);
            return response?.Text ?? string.Empty;
        }

        #endregion xử lý đọc hình ảnh trong file pdf ra text

        /// <summary>
        /// Đọc nội dung text từ danh sách file đính kèm (PDF, Word, Excel, Hình ảnh).
        /// Kết quả sẽ cache vào Redis để lần sau không phải đọc lại.
        /// </summary>
        public async Task<List<ResultReadFileModel>> ReadTextFromFile(List<AttachFileModel> attachFiles)
        {
            int numberOrder = 0;

            var tasks = attachFiles.Select(async attachFile =>
            {
                string filePath = attachFile.AttachURL ?? string.Empty;

                var result = new ResultReadFileModel
                {
                    NumberOrder = Interlocked.Increment(ref numberOrder),
                    FilePath = filePath,
                    AttachID = attachFile.AttachID
                };

                if (!File.Exists(filePath))
                    return result;

                result.FileName = Path.GetFileName(filePath);
                string mimeType = MimeTypesMap.GetMimeType(filePath);
                if (string.IsNullOrEmpty(mimeType))
                    return result;

                var fileInfo = new FileInfo(filePath);
                if (!fileInfo.Exists)
                    return result;

                // Cache key
                string cacheKey = $"filecache:{fileInfo.FullName.ToLowerInvariant()}:{fileInfo.LastWriteTimeUtc.Ticks}:{fileInfo.Length}";

                // Kiểm tra cache
                var cached = await _redisMemoryProvider.GetFileCacheAsync(filePath, cacheKey);
                if (!string.IsNullOrEmpty(cached))
                {
                    result.TextContent = cached;
                    return result;
                }

                try
                {
                    if (ImageMimeTypes.Contains(mimeType))
                    {
                        result.TextContent = await ExtractTextFromImageAsync(filePath);
                    }
                    else
                    {
                        switch (mimeType)
                        {
                            case "application/pdf":
                                result.TextContent = await (
                                    await IsTextPdfWithPdfium(filePath)
                                        ? ReadTextFromPdf(filePath)
                                        : ExtractTextFromPdfAsync(filePath));
                                break;

                            case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                            case "application/msword":
                                result.TextContent = await ExtractTextFromWord(filePath);
                                break;

                            case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                            case "application/vnd.ms-excel":
                                result.TextContent = await ExtractTextFromExcel(filePath);
                                break;
                        }
                    }

                    // Lưu cache nếu có dữ liệu
                    if (!string.IsNullOrEmpty(result.TextContent))
                        await _redisMemoryProvider.SaveFileCacheAsync(filePath, result.TextContent, cacheKey);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                return result;
            });

            var results = await Task.WhenAll(tasks);
            return results.ToList();
        }

        #region hàm xử lý đọc file text loại file Word, Excel, PDF(text)

        private Task<string> ExtractTextFromWord(string filePath)
        {
            using var doc = DocX.Load(filePath);
            return Task.FromResult(doc.Text);
        }

        private Task<string> ExtractTextFromExcel(string filePath)
        {
            using var workbook = new XLWorkbook(filePath);
            var textResult = new StringBuilder();

            foreach (var worksheet in workbook.Worksheets)
            {
                textResult.AppendLine($"--- Sheet: {worksheet.Name} ---");
                foreach (var row in worksheet.RowsUsed())
                {
                    foreach (var cell in row.CellsUsed())
                    {
                        textResult.Append(cell.GetValue<string>());
                        textResult.Append("\t");
                    }
                    textResult.AppendLine();
                }
                textResult.AppendLine();
            }
            return Task.FromResult(textResult.ToString());
        }

        public Task<string> ReadTextFromPdf(string filePath)
        {
            var result = new StringBuilder();
            using (PdfDocument document = PdfDocument.Open(filePath))
            {
                foreach (var page in document.GetPages())
                {
                    var text = page.Text?.Trim();
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        result.AppendLine(text);
                        result.AppendLine("\n---PAGE BREAK---\n");
                    }
                }
            }
            return Task.FromResult(result.ToString());
        }

        #endregion hàm xử lý đọc file text loại file Word, Excel, PDF(text)
    }
}