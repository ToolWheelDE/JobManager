using System.Collections.Generic;

namespace ToolWheel.Extensions.JobManager.Configuration;

public record JobSchedulerDescription(IReadOnlyList<JobSchedulerEntry> Entries) : IJobSchedulerDescription
{ }
