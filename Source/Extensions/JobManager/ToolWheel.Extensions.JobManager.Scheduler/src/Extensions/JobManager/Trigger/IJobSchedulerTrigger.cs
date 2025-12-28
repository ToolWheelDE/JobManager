using System;

namespace ToolWheel.Extensions.JobManager.Trigger;

public interface IJobSchedulerTrigger
{
    bool Enabled { get; set; }

    DateTime NextExecutionTimestamp { get; }

    void UpdateTrigger(DateTime timestamp);
}
