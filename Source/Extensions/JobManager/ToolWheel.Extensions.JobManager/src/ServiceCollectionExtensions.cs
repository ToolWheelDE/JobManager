using System;
using Microsoft.Extensions.DependencyInjection;
using ToolWheel.Extensions.JobManager;
using ToolWheel.Extensions.JobManager.Configuration;
using ToolWheel.Extensions.JobManager.Factory;
using ToolWheel.Extensions.JobManager.Pipeline;
using ToolWheel.Extensions.JobManager.Service;

namespace ToolWheel;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJobManager(this IServiceCollection services, Action<IJobManagerConfigurationBuilder>? builder = null)
    {
        var jobManagerConfigurationBuilder = new JobManagerConfigurationBuilder(services);

        jobManagerConfigurationBuilder.ConfigureMiddleware(configure =>
        {
            //configure.AddLast<JobExceptionHandlerMiddleware>();
            configure.AddLast<JobExecutionMiddleware>();
        });

        jobManagerConfigurationBuilder.ConfigureExecutionCondition(configure =>
        {
            configure.Add<JobMaxInstancesExecutionCondition>();
            configure.Add<JobEnabledExecutionCondition>();
            configure.Add<JobDependenciesExecutionCondition>();
            configure.Add<JobGroupExecutionCondition>();
        });

        builder?.Invoke(jobManagerConfigurationBuilder);
        var jobManagerConfiguration = jobManagerConfigurationBuilder.Build();

        services.AddSingleton<IJobServiceFactory, JobServiceFactory>();
        services.AddSingleton<IJobGroupServiceFactory, JobGroupServiceFactory>();
        services.AddSingleton<IJobServiceConfigurationFactory, JobServiceConfigurationFactory>();
        services.AddSingleton<IJobGroupServiceConfigurationFactory, JobGroupServiceConfigurationFactory>();

        services.AddSingleton<IJobManagerConfiguration>(sp => jobManagerConfiguration);
        services.AddSingleton<IJobTaskJournalService, JobTaskJournalService>();
        services.AddSingleton<IJobTaskService, JobTaskService>();
        services.AddSingleton<IJobInstanceService, JobInstanceService>();
        services.AddSingleton<IJobTaskExecutionService, JobTaskExecutionService>();
        services.AddScoped<IJobTaskContextAccessor, JobTaskContextAccessor>();

        services.AddSingleton(sp =>
        {
            var jobServiceFactory = sp.GetRequiredService<IJobServiceConfigurationFactory>();

            return jobServiceFactory.CreateAndConfigure();
        });

        services.AddSingleton(sp =>
        {
            var jobGroupServiceFactory = sp.GetRequiredService<IJobGroupServiceConfigurationFactory>();

            return jobGroupServiceFactory.CreateAndConfigure();
        });

        foreach (var middleware in jobManagerConfigurationBuilder.ExecutionMiddlewareCollection)
        {
            services.AddScoped(typeof(IExecutionMiddlewareAsync), middleware);
        }

        foreach (var condition in jobManagerConfigurationBuilder.ExecutionConditionCollection)
        {
            services.AddSingleton(typeof(IExecutionCondition), condition);
        }

        return services;
    }
}
