using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebClient.Services;
using WebClient.Services.Interfaces;

namespace WebClient.IOC
{
    public class IoC
    {
        public static void RegisterIoc(IServiceCollection services)
        {
            services.AddSingleton<IChatHubService, ChatHubService>();
        }
    }
}
