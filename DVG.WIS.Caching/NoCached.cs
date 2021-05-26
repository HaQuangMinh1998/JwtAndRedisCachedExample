using DVG.WIS.Caching.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DVG.WIS.Caching
{
    public class NoCached : ICached
    {
        public bool Set<T>(string key, T item, int expireInMinute = 0)
        {
            return false;
        }

        public T Get<T>(string key, HttpContext context = null)
        {
            return default;
        }

        public bool Remove(string key)
        {
            return false;
        }

        public Task<bool> SetAsync<T>(string key, T item, int expireInMinute = 0)
        {
            return Task.FromResult(true);
        }

        public Task<bool> RemoveAsync(string key)
        {
            return Task.FromResult(true);
        }

        public Task<T> GetAsync<T>(string key, HttpContext context = null)
        {
            return Task.FromResult(default(T));
        }

        public Task CloseAsync()
        {
            return Task.FromResult(0);
        }

        public Task<string> GetAsync(string key, HttpContext context = null)
        {
            return Task.FromResult(string.Empty);
        }

        public Task<bool> SetAsync(string key, string item, int expireInMinute = 0)
        {
            return Task.FromResult(true);
        }

        public string Get(string key, HttpContext context = null)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            throw new NotImplementedException();
        }

        public Task<long> EnqueueAsync<T>(string key, T item)
        {
            throw new NotImplementedException();
        }
        public Task<T> DequeueAsync<T>(string key)
        {
            throw new NotImplementedException();
        }
        public Task<bool> SortedSetAddAsync<T>(string key, T item, double score)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> SortedSetRangeByRankAsync<T>(string key)
        {
            throw new NotImplementedException();
        }

        public void RemovePrefix(string keyPattern)
        {
            throw new NotImplementedException();
        }
        public Task<bool> SortedSetRemoveAsync<T>(string key, T item)
        {
            throw new NotImplementedException();
        }
    }
}
