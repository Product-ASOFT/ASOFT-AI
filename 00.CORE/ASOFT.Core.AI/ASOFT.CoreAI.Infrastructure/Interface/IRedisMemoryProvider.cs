using ASOFT.CoreAI.Entities;
using StackExchange.Redis;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IRedisMemoryProvider
    {
        // Lưu dữ liệu người dùng vào Vector Database (Redis Vector Store).
        Task<bool> SaveUserChatToVectorDbAsync(CustomMemoryRecord record, CancellationToken cancellationToken = default);

        // Lấy dữ liệu người dùng từ Vector Database (Redis Vector Store).
        Task<IEnumerable<CustomMemoryRecord>> GetByUserIdAsync(string collectionName, string agentCode, string userId, CancellationToken cancellationToken = default);

        // Lấy thông tin API Key từ Redis.
        Task<bool> IsCheckExistKeyAsync(string cacheKey);

        // Lấy API Key từ Redis.
        Task<string> GetApiKeyAsync(string cacheKey);

        // Lưu API Key vào Redis với thời gian sống (TTL) nhất định.
        Task<string> SaveAPIKeyAsync(string cacheKey, ModelAIChatConfig config, double hour);

        // Lấy cấu hình ModelAI từ Redis.
        Task<ModelAIChatConfig?> GetOpenAIChatConfigAsync(string cacheKey);

        // Lấy cấu hình chat của OpenAI từ Redis.
        Task<bool> CollectionExistsAsync(string indexName);

        // Lưu dữ liệu train cho 1 list dữ liệu.
        Task<IEnumerable<string>> CreateTextSnippetsBatchAsync(string collectionName, IEnumerable<TextSnippet> snippets, CancellationToken cancellationToken = default);

        // Lưu dữ liệu train cho 1 đoạn dữ liệu.
        Task CreateTextSnippetAsync(string collectionName, TextSnippet snippet, CancellationToken cancellationToken = default);

        // Tạo một chỉ mục mới trong Redis Vector Store.
        Task CreateIndexAsync(string operationName, string indexName, CancellationToken cancellationToken = default);

        // tìm kiếm dữ liệu trong Redis Vector Store theo từ khóa.
        Task<RedisResult> SearchByVectorAsync(string indexName, string vectorField, float[] queryEmbedding, int number);

        Task<RedisResult> SearchByKeyOrTextAsync(string indexName, string? keyPrefix = null, int limit = 100, string? keyword = "*");

        Task<string?> GetFileCacheAsync(string filePath, string cacheKey);

        Task SaveFileCacheAsync(string filePath, string textContent, string cacheKey);
    }
}