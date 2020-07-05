using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceNetCore.WindowsService
{
    public static class ServiceBaseLifetimeHostExtensions
    {
        public static IHostBuilder UserServiceBaseLifetime(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices((hostContext, services)=> services.AddSingleton<IHostLifetime, ServiceBaseLifetime>());
        }

        public static Task RunAsServiceAsync(this IHostBuilder hostBuilder, CancellationToken cancellation = default)
        {
            return hostBuilder.UserServiceBaseLifetime().Build().RunAsync(cancellation);
        }
    }
}
