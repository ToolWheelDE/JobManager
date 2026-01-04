using ToolWheel.Extensions.JobManager.Configuration;
using ToolWheel.Extensions.JobManager.Factory;
using ToolWheel.Extensions.JobManager.Pipeline;
using Microsoft.Extensions.DependencyInjection;


namespace ToolWheel;
public static class JobManagerResilienceServiceExtensions
{
    public static IJobManagerConfigurationBuilder UseResilience(this IJobManagerConfigurationBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<IJobResilienceServiceFactory, JobResilienceServiceFactory>();
            services.AddSingleton<IJobResilienceConfigurationFactory, JobResilienceConfigurationFactory>();

            services.AddSingleton(sp =>
            {
                var jobResilienceConfigurationFactory = sp.GetRequiredService<IJobResilienceConfigurationFactory>();
                return jobResilienceConfigurationFactory.CreateAndConfigureJobTaskRetryService();
            });
        })
        .ConfigureMiddleware(middleware =>
        {
            // Das Retry vor dem Exception Handling Middleware ausführen
            middleware.AddBefore<JobTaskRetryMiddleware, JobExecutionMiddleware>();
        });

        return builder;
    }
}
