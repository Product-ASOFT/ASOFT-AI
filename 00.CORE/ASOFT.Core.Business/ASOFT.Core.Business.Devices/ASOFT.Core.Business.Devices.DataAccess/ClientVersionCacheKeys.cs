using ASOFT.Core.Common.InjectionChecker;

namespace ASOFT.Core.Business.Devices.DataAccess
{
    public static class ClientVersionCacheKeys
    {
        private static readonly string MaxVersionCacheKeyPrefix = "memory_cache:asoft:client_management:max_version";

        /// <summary>
        /// Tạo mới cache key cho max version
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        public static string CreateMaxVersionCacheKey(string clientID, string platform)
        {
            Checker.NotEmpty(clientID, nameof(clientID));
            Checker.NotEmpty(platform, nameof(platform));

            return $"{MaxVersionCacheKeyPrefix}.{clientID}.{platform}";
        }
    }
}