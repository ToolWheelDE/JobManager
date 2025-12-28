using System;
using ToolWheel;
using ToolWheel.Extensions.JobManager.Configuration;

namespace ToolWheel;
public static class JobDescriptionBuilderExtensions
{
    public static JobDescriptionBuilder AddScheduler(this JobDescriptionBuilder jobDescriptionBuilder, Action<JobScheduleBuilder> configure)
    {
        var builder = new JobScheduleBuilder();
        configure(builder);

        jobDescriptionBuilder.SetProperty("JobSchedulerDescription", builder.Build());

        return jobDescriptionBuilder;
    }
}
