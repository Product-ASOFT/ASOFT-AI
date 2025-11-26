using ASOFT.CoreAI.Common;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using OpenAI.Embeddings;
using System.Text.Json;

namespace ASOFT.CoreAI.Business
{
    public class OpenAIEmbeddingService : IOpenAIEmbeddingService
    {
        private readonly IRedisMemoryProvider _vectorDatabase;
        private readonly SettingsManagerService _settingsManagerService;

        public OpenAIEmbeddingService(IRedisMemoryProvider vectorDatabase,  SettingsManagerService settingsManagerService)
        {
            _vectorDatabase = vectorDatabase;
            _settingsManagerService = settingsManagerService;
        }
        // Phương thức để tạo embedding từ mô tả văn bản sử dụng OpenAI API
        public async Task<float[]> CreateEmbeddingAsync(string description)
        {
            var cachedKey = await ParseCachedKey();
            if (cachedKey == null || string.IsNullOrWhiteSpace(cachedKey.ApiKey) || string.IsNullOrWhiteSpace(cachedKey.ModelEmbedding))
            {
                Console.WriteLine("Model embedding or API key is not configured.");
                return null;
            }
            EmbeddingClient client = new(cachedKey.ModelEmbedding, cachedKey.ApiKey);

            EmbeddingGenerationOptions options = new() { Dimensions = 1536 };

            OpenAIEmbedding embedding = await client.GenerateEmbeddingAsync(description, options);
            ReadOnlyMemory<float> vector = embedding.ToFloats();
            return vector.ToArray();
        }

        // Phương thức để phân tích và lấy cấu hình ModelAI từ cache

        private async Task<ModelAIChatConfig> ParseCachedKey()
        {
            string cacheKey = AIConstants.ModelAIKey;
            var cachedKey = await _vectorDatabase.GetApiKeyAsync(cacheKey);
            if (cachedKey == null || string.IsNullOrWhiteSpace(cachedKey))
            {
                var configModelAI = await _settingsManagerService.GetModelConfigAI();
                if (string.IsNullOrEmpty(configModelAI.ModelName) || string.IsNullOrEmpty(configModelAI.ApiKey))
                {
                    return new ModelAIChatConfig();
                }
                cachedKey = await _vectorDatabase.GetApiKeyAsync(cacheKey);
            }
            if (cachedKey.StartsWith("{{") && cachedKey.EndsWith("}}"))
            {
                cachedKey = cachedKey.Substring(1, cachedKey.Length - 2);
            }
            // Deserialize JSON
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<ModelAIChatConfig>(cachedKey, options) ?? new ModelAIChatConfig();
        }
    }
}