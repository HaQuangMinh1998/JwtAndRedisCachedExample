using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Business.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Utilities;
using Caching;
using Caching.Interfaces;
using ActionFilter;
using Caching.Configs;

namespace WebAPI.AppStart
{
    public class IOC
    {
        public static void RegisterTypes(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            // Caching
            var cacheType = AppSettings.Instance.GetInt32("CacheType", (int)CachedType.NoCache);
            switch (cacheType)
            {
                case (int)CachedType.Redis:
                    services.AddSingleton<ICached, RedisCached>();
                    break;
                case (int)CachedType.NoCache:
                default:
                    services.AddSingleton<ICached, NoCached>();
                    break;
            }
            services.AddTransient<IUser, User>();


        }
    }
}
