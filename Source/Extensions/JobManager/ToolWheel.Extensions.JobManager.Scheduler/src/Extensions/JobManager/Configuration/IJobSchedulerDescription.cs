using System.Collections.Generic;

namespace ToolWheel.Extensions.JobManager.Configuration;
public interface IJobSchedulerDescription
{
    IReadOnlyList<JobSchedulerEntry> Entries { get; init; }
}