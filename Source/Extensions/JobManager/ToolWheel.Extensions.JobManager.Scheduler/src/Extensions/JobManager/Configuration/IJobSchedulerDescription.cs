using System.Collections.Generic;

namespace ToolWheel.Extensions.JobManager.Configuration;
public interface IJobSchedulerDescription : IFeature
{
    IReadOnlyList<JobSchedulerEntry> Entries { get; init; }
}