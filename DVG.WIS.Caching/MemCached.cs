using Caching.Configs;
using Caching.Interfaces;
using Utilities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Caching
{
    public class MemCached : ICached
    {
        private static string _memcachedIp = AppSettings.Instance.GetString("MemcacedIP");
        private static int _memcachedPort = AppSettings.Instance.GetInt32("MemcachedPort");

        public MemCached()
        {
            // Todo: Init memcaced's configurations
        }
        public bool Set<T>(string key, T item, int expireInMinute)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string key, HttpContext context = null)
        {
            throw new NotImplementedException();
        }

        public string Get(string key, HttpContext context = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetAsync<T>(string key, HttpContext context = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetAsync<T>(string key, T item, int expireInMinute = 0)
        {
            throw new NotImplementedException();
        }

        public static CachedType Key
        {
            get { return CachedType.Memcached; }
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
