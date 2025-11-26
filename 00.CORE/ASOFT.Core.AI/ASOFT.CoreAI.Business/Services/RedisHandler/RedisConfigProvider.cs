using ASOFT.CoreAI.Common;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using ASOFT.CoreAI.Infrastructure.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ASOFT.CoreAI.Common.EnumConstants;

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
                DefaultExpireDays = await _configManagerService.GetConfigIntAsync(APIConfigKeys.REDIS_DEFAULT_EXPIRE_DAYS, 1),
                UserName = await _configManagerService.GetConfigStringAsync(APIConfigKeys.REDIS_DATABASE_USERNAME),
                DatabaseName = await _configManagerService.GetConfigStringAsync(APIConfigKeys.REDIS_DATABASE_NAME),
                Password = await _configManagerService.GetConfigStringAsync(APIConfigKeys.REDIS_DATABASE_PASSWORD)
            };
        }
    }
}
