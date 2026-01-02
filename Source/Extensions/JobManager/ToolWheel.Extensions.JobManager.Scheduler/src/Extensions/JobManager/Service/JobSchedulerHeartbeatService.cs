using System;

namespace ToolWheel.Extensions.JobManager.Service;

public class JobSchedulerHeartbeatService : IJobSchedulerHeartbeatService
{
    private DateTime _lastHeartbeat = DateTime.MinValue;

    public DateTime LastHeartbeat => _lastHeartbeat;

    public void UpdateHeartbeat()
    {
        _lastHeartbeat = DateTime.UtcNow;
    }
}
