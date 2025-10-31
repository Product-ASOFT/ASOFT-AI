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
        private readonly ICIF1640DAL _cif1640DAL;

        public OpenAIEmbeddingService(IRedisMemoryProvider vectorDatabase, ICIF1640DAL cif1640DAL)
        {
            _vectorDatabase = vectorDatabase;
            _cif1640DAL = cif1640DAL;
        }

        public async Task<float[]> CreateEmbeddingAsync(string description)
        {
            var cachedKey = await ParseCachedKey();
            if (cachedKey == null || string.IsNullOrWhiteSpace(cachedKey.ApiKey))
            {
                throw new ArgumentException("Model embedding or API key is not configured.");
            }
            if (string.IsNullOrWhiteSpace(cachedKey.ModelEmbedding))
            {
                cachedKey.ModelEmbedding = "text-embedding-3-small";
            }
            EmbeddingClient client = new(cachedKey.ModelEmbedding, cachedKey.ApiKey);

            EmbeddingGenerationOptions options = new() { Dimensions = 1536 };

            OpenAIEmbedding embedding = await client.GenerateEmbeddingAsync(description, options);
            ReadOnlyMemory<float> vector = embedding.ToFloats();
            return vector.ToArray();
        }

        private async Task<ModelAIChatConfig> ParseCachedKey()
        {
            string cacheKey = AIConstants.ModelAIKey;
            var cachedKey = await _vectorDatabase.GetApiKeyAsync(cacheKey);
            if (cachedKey == null || string.IsNullOrWhiteSpace(cachedKey))
            {
                var configModelAI = await _cif1640DAL.GetConfigModelAI();
                if (!string.IsNullOrEmpty(configModelAI.APIKey) && !string.IsNullOrEmpty(configModelAI.ChatbotModel))
                {
                    double day = 1; // Thời gian lưu trữ key, có thể lấy từ config hoặc tham số
                    var modelAIConfig = new ModelAIChatConfig
                    {
                        ApiKey = configModelAI.APIKey,
                        ModelName = configModelAI.ChatbotModel,
                    };
                    await _vectorDatabase.SaveAPIKeyAsync(cacheKey, modelAIConfig, day);
                    cachedKey = await _vectorDatabase.GetApiKeyAsync(cacheKey);
                }
                else
                {
                    return new ModelAIChatConfig();
                }
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

        //public string BuildText(SimpleTaskInfo task)
        //{
        //    if (task == null) return string.Empty;

        //    return $@"
        //        Mã công việc (TaskID): {task.TaskID}
        //        Tên công việc (TaskName): {task.TaskName}
        //        Loại công việc (TaskType): {task.TaskTypeName}
        //        Người thực hiện (AssignedTo): {task.AssignedToUserName} - {task.AssignedToUserID}
        //        Người hỗ trợ (SupportUser): {task.SupportUserName} - {task.SupportUserID}
        //        Người giám sát (ReviewerUserName) : {task.ReviewerUserName}  - {task.ReviewerUserID}
        //        Trạng thái (Status): {task.StatusName}
        //        Độ ưu tiên (Priority): {task.PriorityName}
        //        Progress: {task.PercentProgress}%
        //        Ngày bắt đầu (PlanStart): {task.PlanStartDate?.ToString("yyyy-MM-dd") ?? ""}
        //        Ngày kết thúc (PlanEnd): {task.PlanEndDate?.ToString("yyyy-MM-dd") ?? ""}
        //        Step: {task.StepName}
        //        TaskHyperlinkedID: {task.TaskHyperlinkedID}
        //        ".Trim();
        //}
    }
}