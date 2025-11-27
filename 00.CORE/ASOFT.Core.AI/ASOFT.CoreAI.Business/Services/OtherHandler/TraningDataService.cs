using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;

namespace ASOFT.CoreAI.Business
{
    public sealed class TrainingDataService : ITrainingDataService
    {
        private readonly IRedisService _redis;
        private readonly SettingsManagerService _settings;

        public TrainingDataService(IRedisService redis, SettingsManagerService settings)
        {
            _redis = redis;
            _settings = settings;
        }

        public async Task<IEnumerable<RedisearchResultItem>> GetTrainingDataAsync(ReadFileRequest request, string indexName)
        {
            var maxRecords = await _settings.GetNumberRecordsAsync();
            return await _redis.GetDataByReadFileAsync(request, indexName, maxRecords.maxTraining) ?? Enumerable.Empty<RedisearchResultItem>();
        }
    }
}