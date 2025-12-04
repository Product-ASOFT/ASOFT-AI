using ASOFT.CoreAI.Common;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using OpenAI.Embeddings;
using System.Text.Json;
using static ASOFT.CoreAI.Common.EnumConstants;

namespace ASOFT.CoreAI.Business
{
    public class OpenAIEmbeddingService : IOpenAIEmbeddingService
    {
        private readonly IRedisMemoryProvider _vectorDatabase;
        private readonly SettingsManagerService _settingsManagerService;
        private readonly ConfigManagerService _configManagerService;

        public OpenAIEmbeddingService(IRedisMemoryProvider vectorDatabase,
            SettingsManagerService settingsManagerService,
            ConfigManagerService configManagerService)
        {
            _vectorDatabase = vectorDatabase;
            _settingsManagerService = settingsManagerService;
            _configManagerService = configManagerService;
        }

        // Phương thức để tạo embedding từ mô tả văn bản sử dụng OpenAI API
        public async Task<float[]?> CreateEmbeddingAsync(string description)
        {
            var cachedKey = await ParseCachedKey();
            if (cachedKey == null || string.IsNullOrWhiteSpace(cachedKey.ApiKey) || string.IsNullOrWhiteSpace(cachedKey.ModelEmbedding))
            {
                return null;
            }
            EmbeddingClient client = new(cachedKey.ModelEmbedding, cachedKey.ApiKey);

            EmbeddingGenerationOptions options = new() { Dimensions = 1536 };

            OpenAIEmbedding embedding = await client.GenerateEmbeddingAsync(description, options);
            ReadOnlyMemory<float> vector = embedding.ToFloats();
            return vector.ToArray();
        }

        // Phương thức để phân tích và lấy cấu hình ModelAI từ cache

        public async Task<ModelAIChatConfig> ParseCachedKey()
        {
            var (config, hasConfig, _, keyStatus, _) = await _settingsManagerService.EnsureModelAIConfigCachedAsync();

            if (hasConfig && keyStatus == AIKeyStatus.Valid)
                return config;

            return new ModelAIChatConfig();
        }
    }
}