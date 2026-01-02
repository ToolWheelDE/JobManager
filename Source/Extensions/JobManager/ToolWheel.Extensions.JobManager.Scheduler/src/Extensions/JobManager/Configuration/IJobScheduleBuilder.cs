using System;
using ToolWheel.Extensions.JobManager.Trigger;

namespace ToolWheel.Extensions.JobManager.Configuration;

public interface IJobScheduleBuilder
{
    /// <summary>
    /// Fügt einen Trigger mit Filterdefinition hinzu.
    /// </summary>
    IJobScheduleBuilder AddTrigger(
        IJobSchedulerTrigger trigger,
        Func<IFilterBuilder, IFilterBuilder>? buildFilter = null);

    /// <summary>
    /// Ohne Filter (AllowAll).
    /// </summary>
    IJobScheduleBuilder AddTrigger(IJobSchedulerTrigger trigger);

    JobSchedulerDescription Build();
}
