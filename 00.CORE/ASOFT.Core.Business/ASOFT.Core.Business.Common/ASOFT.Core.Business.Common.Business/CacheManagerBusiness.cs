using ASOFT.Core.Business.Common.Business.Interfaces;
using ASOFT.Core.Common.InjectionChecker;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Common.Business
{
    public class CacheManagerBusiness : ICacheManagerBusiness
    {
        private readonly IMemoryCache _cache;
        private readonly ConcurrentDictionary<string, HashSet<string>> _cacheKeysByUser = new();

        public CacheManagerBusiness(IMemoryCache cache)
        {
            _cache = Checker.NotNull(cache, nameof(cache));
        }

        public string GetOrAdd(string cacheKey, Func<string> getValue, string userID)
        {
            if (!_cache.TryGetValue(cacheKey, out string result))
            {
                result = getValue();
                SetCache(cacheKey, result, userID);
            }
            return result;
        }

        public async Task<string> GetOrAddAsync(string cacheKey, Func<Task<string>> getValue, string userID)
        {
            if (!_cache.TryGetValue(cacheKey, out string result))
            {
                result = await getValue();
                SetCache(cacheKey, result, userID);
            }
            return result;
        }

        private void SetCache(string cacheKey, string value, string userID)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1),
                SlidingExpiration = TimeSpan.FromHours(8),
                PostEvictionCallbacks = { new PostEvictionCallbackRegistration
            {
                EvictionCallback = (key, value, reason, state) =>
                {
                    if (reason == EvictionReason.Expired || reason == EvictionReason.Removed)
                    {
                        RemoveKeyFromTracking(userID, key.ToString());
                    }
                }
            }}
            };

            _cache.Set(cacheKey, value, cacheEntryOptions);

            _cacheKeysByUser.AddOrUpdate(userID, new HashSet<string> { cacheKey }, (_, keys) =>
            {
                keys.Add(cacheKey);
                return keys;
            });
        }

        public void ResetCacheByUserID(string userID)
        {
            if (_cacheKeysByUser.TryGetValue(userID, out var keys))
            {
                foreach (var key in keys)
                {
                    _cache.Remove(key);
                }
                _cacheKeysByUser.TryRemove(userID, out _);
            }
        }

        private void RemoveKeyFromTracking(string userID, string cacheKey)
        {
            if (_cacheKeysByUser.TryGetValue(userID, out var keys))
            {
                keys.Remove(cacheKey);
                if (keys.Count == 0)
                {
                    _cacheKeysByUser.TryRemove(userID, out _);
                }
            }
        }
    }
}
