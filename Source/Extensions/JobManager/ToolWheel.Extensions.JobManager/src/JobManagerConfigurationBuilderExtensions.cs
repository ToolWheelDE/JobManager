using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ToolWheel.Extensions.JobManager;
using ToolWheel.Extensions.JobManager.Configuration;
using ToolWheel.Extensions.JobManager.Factory;

namespace ToolWheel;
public static class JobManagerConfigurationBuilderExtensions
{
    public static IJobManagerConfigurationBuilder UseJobLogger(this IJobManagerConfigurationBuilder builder, Func<IJob, ILogger> configure)
    {
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<IJobLoggerFactory>(sp => new JobLoggerFactory(configure));
        });

        return builder;
    }
}
