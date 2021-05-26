using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Business.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using DVG.WIS.Utilities;
using DVG.WIS.Caching;
using DVG.WIS.Caching.Interfaces;

namespace WebAPI.AppStart
{
    public class IOC
    {
        public static void RegisterTypes(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            
            services.AddSingleton<ICached, RedisCached>();
            services.AddTransient<IUser, User>();


        }
    }
}
