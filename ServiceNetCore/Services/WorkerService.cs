using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceNetCore.Models;
using ServiceNetCore.Services.TaskQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceNetCore.Services
{
    public class WorkerService : BackgroundService
    {
        private readonly IBackgroundTaskQueue taskQueue;
        private readonly Settings settings;
        private readonly ILogger<WorkerService> logger;

        public WorkerService(ILogger<WorkerService> logger, Settings settings, IBackgroundTaskQueue taskQueue)
        {
            this.taskQueue = taskQueue;
            this.logger = logger;
            this.settings = settings;
        }        

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var workerCount = settings.WorkerCount;
            var workers = Enumerable.Range(0, workerCount).Select(num => RunInstance(num, stoppingToken));

            await Task.WhenAll(workers);
        }

        private async Task RunInstance(int num, CancellationToken stoppingToken)
        {
            logger.LogInformation($"#{num} isv starting.");
            while(!stoppingToken.IsCancellationRequested)
            {
                var workItem = await taskQueue.DequeueAsync(stoppingToken);
                try
                {
                    logger.LogInformation($"{num}: Processing task");
                    await workItem(stoppingToken);
                }
                catch (Exception ex)
                {

                    logger.LogInformation($"{num}: Error task");
                }
                
            }
            logger.LogInformation($"#{num} isv stoping.");
        }
    }
}
