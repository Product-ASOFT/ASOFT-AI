using ASOFT.CoreAI.Entities;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Infrastructure
{
    public interface IRedisConfigProvider
    {
        Task<IConnectionMultiplexer> GetConnectionAsync();
        Task<RedisViewModel> GetRedisConfigAsync(CancellationToken cancellationToken = default);
    }
}
