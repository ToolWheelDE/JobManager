using System;

namespace ToolWheel.Extensions.JobManager.Service;

public interface IJobSchedulerHeartbeatService
{
    DateTime LastHeartbeat { get; }
    void UpdateHeartbeat();
}
