using System;
using ToolWheel.Extensions.JobManager.Configuration;
using ToolWheel.Extensions.JobManager.Trigger;

namespace ToolWheel;
public static class JobScheduleBuilderExtensions
{
    public static JobScheduleBuilder AddInterval(this JobScheduleBuilder builder, TimeSpan interval, Func<IFilterBuilder, IFilterBuilder>? filter = null)
    {
        builder.AddTrigger(new PeriodicTimerTrigger(interval), filter);

        return builder;
    }
}
