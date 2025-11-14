using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Business.Services.BackgroudJobHandler
{
    // ReadFileWorker.cs
    public sealed class ReadFileWorker : BackgroundService
    {
        private readonly IJobQueue _queue;
        private readonly IServiceProvider _sp;
        private readonly ILogger<ReadFileWorker> _logger;

        public ReadFileWorker(IJobQueue queue, IServiceProvider sp, ILogger<ReadFileWorker> logger)
        {
            _queue = queue;
            _sp = sp;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ReadFileWorker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                ReadFileJob job;
                try
                {
                    job = await _queue.DequeueAsync(stoppingToken);
                }
                catch (OperationCanceledException) { break; }

                try
                {
                    using var scope = _sp.CreateScope();
                    var wf = scope.ServiceProvider.GetRequiredService<IReadFileBackgroundWorkflow>();
                    await wf.RunAsync(job.ST2131APK, job.request, job.promptContent, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Job failed {@job}", job);
                }
            }

            _logger.LogInformation("ReadFileWorker stopped.");
        }
    }

}
