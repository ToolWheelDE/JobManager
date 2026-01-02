using ToolWheel.Extensions.JobManager.Filter;
using ToolWheel.Extensions.JobManager.Trigger;

namespace ToolWheel.Extensions.JobManager.Configuration;

public record JobSchedulerEntry(IJobSchedulerTrigger Trigger, IJobSchedulerFilter Filter)
{ }
