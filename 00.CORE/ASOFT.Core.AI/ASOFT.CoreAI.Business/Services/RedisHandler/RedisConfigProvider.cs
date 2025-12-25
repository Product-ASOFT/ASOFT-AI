using ASOFT.CoreAI.Common;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using IronSoftware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace ASOFT.CoreAI.Business
{
    public class RedisConfigProvider : IRedisConfigProvider
    {
        private readonly ConfigManagerService _configManagerService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly object _lock = new();
        private IConnectionMultiplexer? _connection;
        private string? _currentConfigKey;   // hash đơn giản từ ConnectionString + User + Password + DB

        public RedisConfigProvider(ConfigManagerService configManagerService, IServiceScopeFactory scopeFactory)
        {
            _configManagerService = configManagerService;
            _scopeFactory = scopeFactory;
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

        public async Task<IConnectionMultiplexer> GetConnectionAsync()
        {
            // Lazy init hoặc reload khi connection bị mất
            await EnsureConnectionAsync(forceReload: false);
            return _connection!;
        }
        public async Task ReloadAsync()
        {
            // Force reload khi biết config đã đổi
            await EnsureConnectionAsync(forceReload: true);
        }

        private async Task EnsureConnectionAsync(bool forceReload)
        {
            if (!forceReload && _connection != null && _connection.IsConnected)
                return;

            using var scope = _scopeFactory.CreateScope();
            var redisConfigProvider = scope.ServiceProvider.GetRequiredService<IRedisConfigProvider>();
            RedisViewModel redisConfig = await redisConfigProvider.GetRedisConfigAsync();

            if (redisConfig == null || string.IsNullOrWhiteSpace(redisConfig.ConnectionString))
                throw new InvalidOperationException("Redis connection string is missing.");

            // Tạo "key" thể hiện cấu hình hiện tại
            var newConfigKey = $"{redisConfig.ConnectionString}|{redisConfig.UserName}|{redisConfig.Password}|{redisConfig.DatabaseName}";

            // Nếu không force reload và config không đổi, không cần tạo connection mới
            if (!forceReload && newConfigKey == _currentConfigKey && _connection != null && _connection.IsConnected)
                return;

            var options = ConfigurationOptions.Parse(redisConfig.ConnectionString);

            if (!string.IsNullOrEmpty(redisConfig.UserName))
                options.User = redisConfig.UserName;

            if (!string.IsNullOrEmpty(redisConfig.Password))
                options.Password = redisConfig.Password;

            if (!string.IsNullOrEmpty(redisConfig.DatabaseName) &&
                int.TryParse(redisConfig.DatabaseName, out var db))
            {
                options.DefaultDatabase = db;
            }

            options.SyncTimeout = 30000;
            options.AsyncTimeout = 30000;
            options.ConnectTimeout = 30000;
            options.AbortOnConnectFail = false;


            var newConnection = await ConnectionMultiplexer.ConnectAsync(options);

            lock (_lock)
            {
                var old = _connection;
                _connection = newConnection;
                _currentConfigKey = newConfigKey;
                old?.Dispose();
            }

        }
        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}