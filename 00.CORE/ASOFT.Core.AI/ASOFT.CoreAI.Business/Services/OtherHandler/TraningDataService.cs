using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using ASOFT.CoreAI.Infrastructure.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ASOFT.CoreAI.Common.AIConstants;

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
