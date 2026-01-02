using System.Collections.Generic;
using ToolWheel.Extensions.JobManager.Configuration;

namespace ToolWheel.Extensions.JobManager;

public record JobScheduler(IJob Job, IReadOnlyList<JobSchedulerEntry> Entries) : IJobScheduler
{ }
