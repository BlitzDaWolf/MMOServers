using MainServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicServer
{
    public static class AddServices
    {
        public static IHostBuilder AddServerServices<T>(this IHostBuilder builder) where T : BackgroundService
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<Network>();
                services.AddSingleton<ReciveHandler>();

                services.AddHostedService<T>();
            });

            return builder;
        }

        public static IHostBuilder AddClientConnection<T>(this IHostBuilder builder) where T : ClientConnect
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<T>();
            });

            return builder;
        }
    }
}
