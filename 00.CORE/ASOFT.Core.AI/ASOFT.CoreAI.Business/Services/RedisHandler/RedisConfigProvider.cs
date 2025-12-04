using ASOFT.CoreAI.Common;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;

namespace ASOFT.CoreAI.Business
{
    public class RedisConfigProvider : IRedisConfigProvider
    {
        private readonly ConfigManagerService _configManagerService;

        public RedisConfigProvider(ConfigManagerService configManagerService)
        {
            _configManagerService = configManagerService;
        }

        public async Task<RedisViewModel> GetRedisConfigAsync(CancellationToken ct = default)
        {
            return new RedisViewModel
            {
                ConnectionString = await _configManagerService.GetConfigStringAsync(APIConfigKeys.REDIS_CONNECTIONSTRING),
                UserName = await _configManagerService.GetConfigStringAsync(APIConfigKeys.REDIS_DATABASE_USERNAME),
                DatabaseName = await _configManagerService.GetConfigStringAsync(APIConfigKeys.REDIS_DATABASE_NAME),
                Password = await _configManagerService.GetConfigStringAsync(APIConfigKeys.REDIS_DATABASE_PASSWORD)
            };
        }
    }
}