using ASOFT.CoreAI.Common;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using static ASOFT.CoreAI.Common.EnumConstants;

namespace ASOFT.CoreAI.Business
{
    public class ConfigManagerService
    {
        private readonly IConfiguration _configuration;
        private readonly IONT1021Service _ONT1021Queries;
        private readonly IMemoryCache _cache;

        public ConfigManagerService(IConfiguration configuration, IONT1021Service ONT1021Queries, IMemoryCache cache)
        {
            _configuration = configuration;
            _ONT1021Queries = ONT1021Queries;
            _cache = cache;
        }

        // Lấy giá trị cấu hình dạng chuỗi từ bảng ONT1021, nếu không có thì lấy từ appsettings.json
        public async Task<string> GetConfigStringAsync(string key)
        {
            string cacheKey = AIConstants.ONT1021_ALL_SETTINGS;

            // 1. Check cache first
            if (_cache.TryGetValue(cacheKey, out List<ONT1021ViewModel>? cachedSettings) && cachedSettings != null)
            {
                var cachedItem = cachedSettings.FirstOrDefault(x => x.KeyName == key);
                if (cachedItem != null && !string.IsNullOrWhiteSpace(cachedItem.KeyValue))
                    return cachedItem.KeyValue.Trim();
            }

            // 2. Cache không có → query DB
            List<int> categoryIDs = new()
            {
                (int)IntegrationServiceType.EXTERNAL_SYSTEM_CONNECTION
            };
            try
            {
                var settingsFromDb = await _ONT1021Queries.GetAllAsync(categoryIDs);

                // 3. Lưu vào cache nếu DB có dữ liệu
                if (settingsFromDb != null && settingsFromDb.Any())
                {
                    _cache.Set(cacheKey, settingsFromDb,
                        new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                            SlidingExpiration = TimeSpan.FromMinutes(5)
                        });

                    // Lấy giá trị config theo key
                    var itemDb = settingsFromDb.FirstOrDefault(x => x.KeyName == key);
                    if (itemDb != null && !string.IsNullOrWhiteSpace(itemDb.KeyValue))
                        return itemDb.KeyValue.Trim();
                }
            }
            catch (Exception)
            {
                // 4. TH DB không có tạo module ON
                return GetConfigAppSetting(key);
            }
            // 5. TH có tạo module ON nhưng không có giá trị cấu hình.
            return GetConfigAppSetting(key);
        }
        // Hàm lấy giá trị từ Appsetting
        private string GetConfigAppSetting(string key)
        {
            // Nếu DB không có → lấy từ appsettings
            var cfg = _configuration.GetValue<string>($"AI_CONFIG:{key}");
            if (!string.IsNullOrWhiteSpace(cfg))
                return cfg.Trim();
            return string.Empty;
        }
        // Lấy giá trị cấu hình dạng số nguyên từ bảng ONT1021, nếu không có thì lấy từ appsettings.json
        public async Task<int> GetConfigIntAsync(string key, int defaultValue = 5)
        {
            int result = defaultValue;

            string raw = await GetConfigStringAsync(key);
            if (!string.IsNullOrEmpty(raw))
            {
                int parsed;
                if (int.TryParse(raw, out parsed))
                {
                    result = parsed;
                }
            }
            return result;
        }

        // Lấy giá trị cấu hình dạng boolean từ bảng ONT1021, nếu không có thì lấy từ appsettings.json
        public async Task<bool> GetConfigBoolAsync(string key, bool defaultValue = false)
        {
            bool result = defaultValue;

            string raw = await GetConfigStringAsync(key);

            if (!string.IsNullOrEmpty(raw))
            {
                bool parsed;
                if (bool.TryParse(raw, out parsed))
                {
                    result = parsed;
                }
            }

            return result;
        }
    }
}