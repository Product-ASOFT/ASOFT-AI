using System;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Common.Business.Interfaces
{
    public interface ICacheManagerBusiness
    {
        string GetOrAdd(string cacheKey, Func<string> getValue, string userID);
        Task<string> GetOrAddAsync(string cacheKey, Func<Task<string>> getValue, string userID);
        void ResetCacheByUserID(string userID);
    }
}
