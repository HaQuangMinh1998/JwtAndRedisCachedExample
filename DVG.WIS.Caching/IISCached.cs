using DVG.WIS.Caching.Configs;
using DVG.WIS.Caching.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace DVG.WIS.Caching
{
    public class IISCached : ICached, IDisposable
    {
        private readonly IMemoryCache _cache;
        public IISCached()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
        }
        public IISCached(IMemoryCache cache)
        {
            this._cache = cache;
        }

        public bool Set<T>(string key, T item, int expireInMinute)
        {
            try
            {
                if (item == null || string.IsNullOrEmpty(key)) return false;

                _cache.Set(key, item, DateTime.Now.AddMinutes(expireInMinute));

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string Get(string key, HttpContext context = null)
        {
            throw new NotImplementedException();
        }

        public bool Set<T>(string key, T item, FileCacheDependency dependency)
        {
            try
            {
                if (item == null || string.IsNullOrEmpty(key)) return false;

                var cacheEntryOptions = new MemoryCacheEntryOptions().AddExpirationToken(dependency.FileWatch);
                _cache.Set(key, item, cacheEntryOptions);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Remove(string key)
        {
            try
            {
                if (string.IsNullOrEmpty(key)) return true;

                _cache.Remove(key);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public T Get<T>(string key, HttpContext context = null)
        {
            try
            {
                return _cache.Get<T>(key);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        public static CachedType Key
        {
            get { return CachedType.IIS; }
        }


        public void Dispose()
        {
            // Do nothing
        }

        public async Task<T> GetAsync<T>(string key, HttpContext context = null)
        {
            try
            {
                return await Task.FromResult(Get<T>(key));
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        public async Task<bool> SetAsync<T>(string key, T item, int expireInMinute)
        {
            try
            {
                if (item == null || string.IsNullOrEmpty(key)) return false;

                await Task.FromResult(Set(key, item, expireInMinute));

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ContainsKey(string key)
        {
            throw new NotImplementedException();
        }
    }
}
