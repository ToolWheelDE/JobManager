using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ToolWheel.Extensions.JobManager.Service;

public class JobSchedulerHealthCheck : IHealthCheck
{
    private readonly IJobSchedulerHeartbeatService _jobSchedulerHeartbeatService;

    public JobSchedulerHealthCheck(IJobSchedulerHeartbeatService jobSchedulerHeartbeatService)
    {
        _jobSchedulerHeartbeatService = jobSchedulerHeartbeatService;
    }


    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var timeSinceLastHeartbeat = DateTime.UtcNow - _jobSchedulerHeartbeatService.LastHeartbeat;

        if (timeSinceLastHeartbeat < TimeSpan.FromSeconds(10))
        {
            return Task.FromResult(HealthCheckResult.Healthy("JobScheduler is running."));
        }

        return Task.FromResult(HealthCheckResult.Unhealthy($"No heartbeat for {timeSinceLastHeartbeat.TotalSeconds:F1} seconds."));

    }
}
