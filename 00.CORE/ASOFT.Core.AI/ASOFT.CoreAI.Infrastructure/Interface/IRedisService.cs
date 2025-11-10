using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Entities.ViewModels.AI;
using Newtonsoft.Json.Linq;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IRedisService
    {
        // Lấy dữ liệu đã được training từ Redis theo câu hỏi và tên chỉ mục.
        Task<List<JObject>> GetDataTrainFormRedis(string question, string indexName, int number);

        Task<IEnumerable<RedisearchResultItem>> GetDataTrainingAsync(AgentRequest agentRequest, string indexName, int number);

        Task<List<RedisearchResultItem>> GetDataByReadFileAsync(AgentRequest agentRequest, string indexName, int number);

        Task<List<RedisearchResultItem>> GetDataByReadFileAsync(ReadFileRequest agentRequest, string indexName, int number);
    }
}