using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ToolWheel.Extensions.JobManager.Service;

public class JobSchedulerWatchdogService : BackgroundService
{
    private readonly ILogger<JobSchedulerWatchdogService> _logger;
    private readonly HealthCheckService _healthCheckService;
    private readonly IHostApplicationLifetime _lifetime;

    public JobSchedulerWatchdogService(ILogger<JobSchedulerWatchdogService> logger, HealthCheckService healthCheckService, IHostApplicationLifetime lifetime)
    {
        _logger = logger;
        _healthCheckService = healthCheckService;
        _lifetime = lifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Monitoring JobScheduler health...");

        var timer = new PeriodicTimer(TimeSpan.FromSeconds(2));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            var report = await _healthCheckService.CheckHealthAsync(_ => true, stoppingToken);

            if (report.Status != HealthStatus.Healthy)
            {
                _logger.LogError("JobScheduler unhealthy for too long. Stopping application to trigger restart.");
                Environment.Exit(1); // ExitCode 1 indicates an error condition, which will trigger a restart in many hosting environments.
            }
        }

        _logger.LogInformation("Job-Scheduler Watchdog stopped.");
    }
}
