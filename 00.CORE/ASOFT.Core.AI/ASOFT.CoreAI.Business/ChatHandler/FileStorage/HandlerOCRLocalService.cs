using Microsoft.AspNetCore.StaticFiles;
using System.Net.Http.Headers;


namespace ASOFT.CoreAI.Business.ChatHandler.FileStorage
{
    public class HandlerOCRLocalService
    {
        private readonly SettingsManager _settingsManager;
        private static readonly HttpClient _httpClient = new HttpClient();
        public HandlerOCRLocalService(SettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }
        // Hàm chuyển đổi từ file hình ảnh sang text sử dụng dịch vụ OCR local
        public async Task<string> ReadFileOCRWithLocal(List<string> FilePaths)
        {
            string ocrUrl = _settingsManager.GetUrlReadOCR();
            if ("" == ocrUrl)
                throw new Exception("Chưa cấu hình URL OCR");
            using var form = new MultipartFormDataContent();

            var provider = new FileExtensionContentTypeProvider();

            foreach (var filePath in FilePaths)
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"Không tìm thấy file: {filePath}");

                var fileStream = File.OpenRead(filePath);
                var fileContent = new StreamContent(fileStream);

                // Lấy content-type dựa theo phần mở rộng
                if (!provider.TryGetContentType(filePath, out var contentType))
                    contentType = "application/octet-stream"; // fallback

                fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                form.Add(fileContent, "files", Path.GetFileName(filePath));
            }
            var response = await _httpClient.PostAsync(ocrUrl, form);

            if (!response.IsSuccessStatusCode)
                response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return result ?? string.Empty;
        }
    }
}
