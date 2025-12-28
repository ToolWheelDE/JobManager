using System.Collections.Generic;
using ToolWheel.Extensions.JobManager.Configuration;

namespace ToolWheel.Extensions.JobManager;

public interface IJobScheduler 
{
    IReadOnlyList<JobSchedulerEntry> Entries { get; init; }

    IJob Job { get; init; }
}
