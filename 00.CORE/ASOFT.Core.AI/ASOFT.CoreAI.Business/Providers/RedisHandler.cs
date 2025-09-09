using ASOFT.CoreAI.Common;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ASOFT.CoreAI.Business
{
    public class RedisHandler : IRedisHandler
    {
        private readonly IRedisMemoryProvider _vectorDatabase;
        private readonly IOpenAIEmbeddingService _embeddingService;

        public RedisHandler(IRedisMemoryProvider vectorDatabase, IOpenAIEmbeddingService openAIEmbeddingService)
        {
            _embeddingService = openAIEmbeddingService;
            _vectorDatabase = vectorDatabase;
        }

        public async Task<List<JObject>> GetDataTrainFormRedis(string question, string indexName, int number)
        {
            float[] queryEmbedding = await _embeddingService.CreateEmbeddingAsync(question);
            var result = await _vectorDatabase.SearchByVectorAsync(indexName, AIConstants.FIELD_EMBEDDING, queryEmbedding, number);
            if (result == null || result.IsNull)
            {
                return new List<JObject>();
            }
            List<RedisearchResultItem> items = RedisearchResultParser.Parse(result);
            if (items == null || items.Count == 0)
            {
                return new List<JObject>();
            }
            List<JObject> jsonObjects = items
            .Select(item => JObject.Parse(JsonConvert.SerializeObject(item)))
            .ToList();
            // Hoặc lấy JSON string
            //string json = RedisearchResultParser.ToJson(result);
            return jsonObjects;
        }

        public async Task<IEnumerable<RedisearchResultItem>> GetDataTrainingAsync(AgentRequest agentRequest, string indexName, int number)
        {
            var jsonObjects = await GetDataTrainFormRedis(agentRequest.Question, indexName, number);
            if (jsonObjects == null || jsonObjects.Count == 0)
            {
                return new List<RedisearchResultItem>();
            }
            var items = jsonObjects
                .Select(item => item.ToObject<RedisearchResultItem>())
                .Where(item => item != null) // Filter out null items
                .ToList();
            return items!;
        }

        public async Task<List<RedisearchResultItem>> GetDataByReadFileAsync(AgentRequest agentRequest, string indexName, int number)
        {
            var jsonObjects = await _vectorDatabase.SearchByKeyOrTextAsync(indexName, agentRequest.AgentCode, number);
            List<RedisearchResultItem> items = RedisearchResultParser.Parse(jsonObjects);
            if (items == null || items.Count == 0)
            {
                return new List<RedisearchResultItem>();
            }
            return items!;
        }

        public async Task<List<RedisearchResultItem>> GetDataByReadFileAsync(ReadFileRequest agentRequest, string indexName, int number)
        {
            var jsonObjects = await _vectorDatabase.SearchByKeyOrTextAsync(indexName, agentRequest.AgentCode, number);
            List<RedisearchResultItem> items = RedisearchResultParser.Parse(jsonObjects);
            if (items == null || items.Count == 0)
            {
                return new List<RedisearchResultItem>();
            }
            return items!;
        }
    }
}