using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ToolWheel.Extensions.JobManager.Configuration;

namespace ToolWheel.Extensions.JobManager.Service;

public class JobSchedulerHostedService : BackgroundService
{
    private readonly ILogger<JobSchedulerHostedService>? _logger;
    private readonly IJobSchedulerService _jobSchedulerService;
    private readonly IJobSchedulerHeartbeatService _jobSchedulerHeartbeatService;
    private readonly IJobTaskService _jobTaskService;

    public JobSchedulerHostedService(IJobSchedulerService jobSchedulerService, IJobSchedulerHeartbeatService jobSchedulerHeartbeatService, IJobTaskService jobTaskService, ILogger<JobSchedulerHostedService>? logger = null)
    {
        _logger = logger;
        _jobSchedulerService = jobSchedulerService;
        _jobSchedulerHeartbeatService = jobSchedulerHeartbeatService;
        _jobTaskService = jobTaskService;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _jobTaskService.WaitAllTasks();

        _logger?.LogInformation("Job-Scheduler service is stopped.");

        return base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        _logger?.LogInformation("Job-Scheduler service is starting.");

        _jobSchedulerHeartbeatService.UpdateHeartbeat();

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            var scheduledJobs = _jobSchedulerService.GetJobSchedulers();
            var signalDate = DateTime.UtcNow;

            foreach (var jobScheduler in scheduledJobs)
            {
                var triggers = jobScheduler.Entries.Where(m => CheckExecution(m, signalDate));

                if (triggers.Any())
                {
                    _jobTaskService.Execute(jobScheduler.Job);
                }

                foreach (var trigger in triggers)
                {
                    trigger.Trigger.UpdateTrigger(signalDate);
                }
            }

            _jobSchedulerHeartbeatService.UpdateHeartbeat();
        }

        _logger?.LogInformation("Job-Scheduler service is stopping.");
    }

    private bool CheckExecution(JobSchedulerEntry jobSchedulerEntry, DateTime signalDate)
    {
        return jobSchedulerEntry.Trigger.Enabled &&
               jobSchedulerEntry.Trigger.NextExecutionTimestamp <= signalDate &&
               jobSchedulerEntry.Filter.CheckExecutable(signalDate);
    }
}
