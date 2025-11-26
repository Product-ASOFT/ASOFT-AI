using ASOFT.CoreAI.Infrastructure;
using Microsoft.Extensions.Configuration;
using static ASOFT.CoreAI.Common.EnumConstants;

namespace ASOFT.CoreAI.Business
{
    public class ConfigManagerService
    {
        private readonly IConfiguration _configuration;
        private readonly IONT1021Service _ONT1021Queries;

        public ConfigManagerService(IConfiguration configuration, IONT1021Service ONT1021Queries)
        {
            _configuration = configuration;
            _ONT1021Queries = ONT1021Queries;
        }
        public async Task<string> GetConfigStringAsync(string key)
        {
            string value = string.Empty;
            List<int> CategoryIDs = new List<int>()
            {
                (int)IntegrationServiceType.EXTERNAL_SYSTEM_CONNECTION
            };
            var settings = await _ONT1021Queries.GetAllAsync(CategoryIDs);

            if (settings != null && settings.Any())
            {
                var item = settings.FirstOrDefault(x => x.KeyName == key);
                if (item != null && !string.IsNullOrWhiteSpace(item.KeyValue))
                {
                    value = item.KeyValue.Trim();
                }
            }

            if (string.IsNullOrEmpty(value))
            {
                var cfg = _configuration.GetValue<string>("AI_CONFIG:" + key);
                if (!string.IsNullOrEmpty(cfg))
                {
                    value = cfg;
                }
            }
            return value;
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
