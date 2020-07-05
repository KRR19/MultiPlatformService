﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceNetCore.Models;
using ServiceNetCore.Services.TaskQueue;
using ServiceNetCore.Worker;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceNetCore.Services
{
    public class TaskSchedulerService : IHostedService, IDisposable
    {
        private Timer timer;
        private readonly IServiceProvider service;
        private readonly Settings settings;
        private readonly ILogger logger;
        private readonly Random random = new Random();
        private readonly object syncRoot = new object();

        public TaskSchedulerService(IServiceProvider service)
        {
            this.service = service;
            this.settings = service.GetRequiredService<Settings>();
            this.logger = service.GetRequiredService<ILogger<TaskSchedulerService>>();
        }
        private void ProcessTask()
        {
            if (Monitor.TryEnter(syncRoot))
            {
                logger.LogInformation($"Process task started");
                DoWork();
                logger.LogInformation($"Process task finished");
                Monitor.Exit(syncRoot);
            }
            else
            {
                logger.LogInformation($"Procces in currently in progress");
            }
        }

        private void DoWork()
        {
            var number = random.Next(20);
            var proccessor = service.GetRequiredService<TaskProcessor>();
            var queue = service.GetRequiredService<IBackgroundTaskQueue>();
            queue.QueueBackgroundWorkItem(token =>
            {
                return proccessor.RunAsync(number, token);
            });
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var interval = settings?.RunInterval ?? 0;
            if (interval == 0)
            {
                logger.LogWarning("CheckInterval");
                interval = 60;
            }
            timer = new Timer(
                (e) => ProcessTask(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(interval)
                );
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
