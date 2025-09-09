using ASOFT.CoreAI.Common;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using ClosedXML.Excel;
using System.Data;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;

namespace ASOFT.CoreAI.Business
{
    public sealed class DataLoader : IDataLoader
    {
        private readonly IRedisMemoryProvider _vectorDatabase;
        private readonly IOpenAIEmbeddingService _embeddingService;

        public DataLoader(IRedisMemoryProvider vectorDatabase, IOpenAIEmbeddingService openAIEmbeddingService)
        {
            _vectorDatabase = vectorDatabase;
            _embeddingService = openAIEmbeddingService;
        }

        public async Task LoadTrainingDataFromDocument(LoadFileRequest request, CancellationToken cancellationToken)
        {
            string extension = Path.GetExtension(request.FilePath).ToLowerInvariant();
            IEnumerable<RawContent> sections;
            switch (extension)
            {
                case ".pdf":
                    sections = LoadTextAndImagesFromPDF(request.FilePath, cancellationToken, null);
                    break;

                case ".docx":
                case ".doc":
                    sections = LoadTextAndImagesFromWord(request.FilePath, cancellationToken);
                    break;

                case ".xlsx":
                case ".xls":
                    sections = LoadTextFromExcel(request.FilePath, cancellationToken);
                    break;

                default:
                    throw new NotSupportedException($"File type {extension} is not supported.");
            }
            // Create the collection if it doesn't exist.
            string indexName = AgentKeyHelper.GetIndexKey(request.IndexName);
            try
            {
                await CreateIndexInRedis(indexName, cancellationToken);
            }
            catch (Exception)
            {
                throw;
            }
            // Load the text and images from the PDF file and split them into batches.
            var batches = sections.Chunk(request.BatchSize);
            var records = new List<TextSnippet>();

            // Process each batch of content items.
            foreach (var batch in batches)
            {
                var textContents = await Task.WhenAll(batch.Select(content =>
                {
                    if (!string.IsNullOrWhiteSpace(content.Text))
                    {
                        return Task.FromResult(content);
                    }
                    return Task.FromResult(new RawContent { Text = string.Empty, PageNumber = content.PageNumber });
                }));
                foreach (var content in textContents)
                {
                    if (string.IsNullOrWhiteSpace(content.Text))
                        continue; // bỏ qua đoạn text rỗng

                    float[] embeddingVector = await _embeddingService.CreateEmbeddingAsync(content.Text);

                    records.Add(new TextSnippet
                    {
                        Key = Guid.NewGuid(),
                        Text = content.Text,
                        ReferenceDescription = $"{Path.GetFileName(request.FilePath)}#page={content.PageNumber}",
                        ReferenceLink = $"{new Uri(Path.GetFullPath(request.FilePath)).AbsoluteUri}#page={content.PageNumber}",
                        EmbeddingVector = embeddingVector,
                        FileType = extension,
                        CreatedAt = DateTime.Now,
                    });
                }
            }
            await _vectorDatabase.CreateTextSnippetsBatchAsync(indexName, records, cancellationToken: cancellationToken);
            await Task.Delay(request.BetweenBatchDelayInMs, cancellationToken).ConfigureAwait(false);
        }

        #region

        // Load text and images from PDF file.
        private IEnumerable<RawContent> LoadTextAndImagesFromPDF(string? pdfPath, CancellationToken cancellationToken, MemoryStream? ms)
        {
            // Mở tài liệu từ path hoặc MemoryStream
            using var document = !string.IsNullOrEmpty(pdfPath)
                ? PdfDocument.Open(pdfPath)
                : PdfDocument.Open(ms ?? throw new ArgumentNullException(nameof(ms)));

            foreach (var page in document.GetPages())
            {
                if (cancellationToken.IsCancellationRequested) yield break;

                // Ảnh
                foreach (var image in page.GetImages())
                {
                    if (cancellationToken.IsCancellationRequested) yield break;

                    if (image.TryGetPng(out var png))
                    {
                        yield return new RawContent { Image = png, PageNumber = page.Number };
                    }
                    else
                    {
                        Console.WriteLine($"Unsupported image format on page {page.Number}");
                    }
                }

                // Text
                var blocks = DefaultPageSegmenter.Instance.GetBlocks(page.GetWords());
                foreach (var block in blocks)
                {
                    if (cancellationToken.IsCancellationRequested) yield break;

                    yield return new RawContent { Text = block.Text, PageNumber = page.Number };
                }
            }
        }

        // Load text and images from Word file (DOCX or DOC).
        private IEnumerable<RawContent> LoadTextAndImagesFromWord(string wordPath, CancellationToken cancellationToken)
        {
            // Kiểm tra file có tồn tại không
            if (string.IsNullOrWhiteSpace(wordPath) || !File.Exists(wordPath))
                throw new FileNotFoundException("Đường dẫn file Word không hợp lệ.", wordPath);

            using var ms = new MemoryStream();

            try
            {
                var doc = new Aspose.Words.Document(wordPath); // có thể mở cả .doc và .docx
                doc.Save(ms, Aspose.Words.SaveFormat.Pdf);

                ms.Position = 0;
                return LoadTextAndImagesFromPDF(null, cancellationToken, ms).ToList();
            }
            catch (Aspose.Words.UnsupportedFileFormatException ex)
            {
                throw new InvalidOperationException("File không đúng định dạng Word được hỗ trợ.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi chuyển đổi Word sang PDF: {ex.Message}", ex);
            }
            finally
            {
                ms.Dispose(); // ✅ chỉ dispose sau khi enumerate xong
            }
        }

        // Load text from Excel file (XLSX or XLS).
        private IEnumerable<RawContent> LoadTextFromExcel(string excelPath, CancellationToken cancellationToken)
        {
            using var workbook = new XLWorkbook(excelPath);

            int sheetNumber = 1;

            foreach (var worksheet in workbook.Worksheets)
            {
                if (cancellationToken.IsCancellationRequested)
                    yield break;

                // Lấy toàn bộ dòng có dữ liệu trong sheet
                var rows = worksheet.RangeUsed()?.RowsUsed();

                if (rows == null)
                    continue;

                bool firstRow = true;
                foreach (var row in rows)
                {
                    if (cancellationToken.IsCancellationRequested)
                        yield break;

                    if (firstRow)
                    {
                        // Bỏ qua hàng tiêu đề đầu tiên
                        firstRow = false;
                        continue;
                    }

                    // Lấy các ô trong hàng, bỏ cột 1, chỉ lấy từ cột 2 trở đi
                    // Giả sử cột 2 là index 2 (1-based)
                    string firstCellValue = row.Cell(2).GetValue<string>().Trim();

                    // Lấy giá trị ô thứ hai (cột 2)
                    string secondCellValue = row.Cell(3).GetValue<string>().Trim();

                    // Tạo chuỗi JSON bọc ô thứ 2 trong dấu {}
                    string jsonString = "{" + secondCellValue + "}";

                    // Nối chuỗi: giá trị ô 1 + tab + jsonString
                    string rowText = $"{firstCellValue}\t{jsonString}";

                    if (!string.IsNullOrWhiteSpace(rowText))
                    {
                        yield return new RawContent
                        {
                            Text = rowText,
                            PageNumber = sheetNumber  // Dùng sheetNumber làm PageNumber giả định
                        };
                    }
                }

                sheetNumber++;
            }
        }

        #endregion

        #region tạo chỉ mục

        // Tạo chỉ mục trong Redis Vector Store nếu chưa tồn tại.
        private async Task CreateIndexInRedis(string indexName, CancellationToken cancellationToken)
        {
            bool IsCreateIndex = await _vectorDatabase.CollectionExistsAsync(indexName).ConfigureAwait(false);
            if (!IsCreateIndex)
            {
                await _vectorDatabase.CreateIndexAsync(AIConstants.CreateIndex, indexName, cancellationToken).ConfigureAwait(false);
            }
        }

        #endregion
    }
}