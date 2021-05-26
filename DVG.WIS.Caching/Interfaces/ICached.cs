using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Caching.Interfaces
{
    public interface ICached
    {
        bool Set<T>(string key, T item, int expireInMinute = 0);
        bool Remove(string key);
        T Get<T>(string key, HttpContext context = null);
        string Get(string key, HttpContext context = null);
        Task<T> GetAsync<T>(string key, HttpContext context = null);
        Task<bool> SetAsync<T>(string key, T item, int expireInMinute = 0);
        bool ContainsKey(string key);
        Task<long> EnqueueAsync<T>(string key, T item);
        Task<T> DequeueAsync<T>(string key);
        Task<bool> SortedSetAddAsync<T>(string key, T item, double score);
        Task<List<T>> SortedSetRangeByRankAsync<T>(string key);
        void RemovePrefix(string keyPattern);
        Task<bool> SortedSetRemoveAsync<T>(string key, T item);
    }
}
