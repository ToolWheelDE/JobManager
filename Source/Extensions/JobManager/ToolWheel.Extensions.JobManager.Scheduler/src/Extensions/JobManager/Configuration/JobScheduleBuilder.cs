using System;
using System.Collections.Generic;
using ToolWheel.Extensions.JobManager.Trigger;

namespace ToolWheel.Extensions.JobManager.Configuration;

public sealed class JobScheduleBuilder : IJobScheduleBuilder
{
    private readonly List<JobSchedulerEntry> _entries = new();

    public JobScheduleBuilder()
    { }

    public IJobScheduleBuilder AddTrigger(IJobSchedulerTrigger trigger, Func<IFilterBuilder, IFilterBuilder>? buildFilter = null)
    {
        if (trigger is null)
        {
            throw new ArgumentNullException(nameof(trigger));
        }

        var fb = new FilterBuilder();
        var finalFilter = (buildFilter != null ? buildFilter(fb) : fb).Build();

        _entries.Add(new JobSchedulerEntry(trigger, finalFilter));

        return this;
    }

    public IJobScheduleBuilder AddTrigger(IJobSchedulerTrigger trigger)
    {
        return AddTrigger(trigger, null);
    }

    public JobSchedulerDescription Build()
    {
        return new JobSchedulerDescription(_entries);
    }
}
