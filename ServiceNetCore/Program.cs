using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceNetCore.Extansions.HostExtansions;
using ServiceNetCore.Models;
using ServiceNetCore.Services;
using ServiceNetCore.Services.TaskQueue;
using ServiceNetCore.Worker;
using System;
using System.Threading.Tasks;

namespace ServiceNetCore
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration(confBuilder =>
                {
                    confBuilder.AddJsonFile("config.json");
                    confBuilder.AddCommandLine(args);
                })
                .ConfigureLogging((configLogging) =>
                {
                    configLogging.AddConsole();
                    configLogging.AddDebug();
                })
                .ConfigureServices((services) =>
                {
                    services.AddHostedService<TaskSchedulerService>();
                    services.AddHostedService<WorkerService>();

                    services.AddSingleton<Settings>();
                    services.AddSingleton<TaskProcessor>();
                    services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
                });



            await builder.RunService();
        }
    }
}
