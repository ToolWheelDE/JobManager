using Microsoft.Extensions.DependencyInjection;
using ToolWheel.Extensions.JobManager.Configuration;
using ToolWheel.Extensions.JobManager.Factory;
using ToolWheel.Extensions.JobManager.Service;

namespace ToolWheel;
public static class JobManagerConfigurationBuilderExtensions
{
    public static IJobManagerConfigurationBuilder UseJobManagerSchedulerWatchDog(this IJobManagerConfigurationBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddHostedService<JobSchedulerWatchdogService>();
        });

        return builder;
    }

    public static IJobManagerConfigurationBuilder UseJobManagerScheduler(this IJobManagerConfigurationBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<IJobSchedulerServiceFactory, JobSchedulerServiceFactory>();
            services.AddSingleton<IJobSchedulerCreateAndConfigurationFactory, JobSchedulerCreateAndConfigurationFactory>();
            services.AddSingleton<IJobSchedulerHeartbeatService, JobSchedulerHeartbeatService>();

            services.AddHealthChecks()
                .AddCheck<JobSchedulerHealthCheck>("JobManager_Scheduler_HealthCheck");

            services.AddHostedService<JobSchedulerHostedService>();

            services.AddSingleton(sp =>
            {
                var jobSchedulerServiceFactory = sp.GetRequiredService<IJobSchedulerCreateAndConfigurationFactory>();

                return jobSchedulerServiceFactory.CreateAndConfigure();
            });
        });

        return builder;
    }
}
