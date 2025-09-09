using ASOFT.CoreAI.Entities;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IOpenAIEmbeddingService
    {
        // Tạo dữ liệu embedding từ mô tả văn bản
        Task<float[]> CreateEmbeddingAsync(string description);

        //string BuildText(SimpleTaskInfo task);
    }
}