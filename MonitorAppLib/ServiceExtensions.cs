using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MonitorAppLib.Clients;
using MonitorAppLib.Services;

namespace MonitorAppLib
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddSendDelayService(this IServiceCollection collection)
        {
            return collection
                .AddTransient<ISendDelay, SendDelayService>()
                .ClientsRegistrar();
        }
        private static IServiceCollection ClientsRegistrar(this IServiceCollection services)
        {
            services.AddHttpClient<DelayClient>();
            return services;
        }
    }
}
