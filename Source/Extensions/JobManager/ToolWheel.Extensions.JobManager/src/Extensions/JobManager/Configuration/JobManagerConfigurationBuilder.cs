using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace ToolWheel.Extensions.JobManager.Configuration;
public class JobManagerConfigurationBuilder : IJobManagerConfigurationBuilder
{
    private readonly IServiceCollection _serviceCollection;

    public JobManagerConfigurationBuilder(IServiceCollection serviceCollection)
    {
        JobDescriptionCollection = new JobDescriptionCollection();
        JobGroupDescriptionCollection = new JobGroupDescriptionCollection();
        ExecutionMiddlewareCollection = new ExecutionMiddlewareCollection();
        ExecutionConditionCollection = new ExecutionConditionCollection();

        _serviceCollection = serviceCollection;
    }

    public IJobManagerConfigurationBuilder ConfigureJobs(Action<JobDescriptionCollection> configure)
    {
        configure?.Invoke(JobDescriptionCollection);

        return this;
    }

    public IJobManagerConfigurationBuilder ConfigureGroups(Action<JobGroupDescriptionCollection> configure)
    {
        configure?.Invoke(JobGroupDescriptionCollection);

        return this;
    }

    public IJobManagerConfigurationBuilder ConfigureMiddleware(Action<ExecutionMiddlewareCollection> configure)
    {
        configure?.Invoke(ExecutionMiddlewareCollection);
        return this;
    }

    public IJobManagerConfigurationBuilder ConfigureExecutionCondition(Action<ExecutionConditionCollection> configure)
    {
        configure?.Invoke(ExecutionConditionCollection);
        return this;
    }

    public IJobManagerConfigurationBuilder ConfigureServices(Action<IServiceCollection> services)
    {
        services?.Invoke(_serviceCollection);
        return this;
    }

    public JobDescriptionCollection JobDescriptionCollection { get; }

    public JobGroupDescriptionCollection JobGroupDescriptionCollection { get; }

    public ExecutionMiddlewareCollection ExecutionMiddlewareCollection { get; }

    public ExecutionConditionCollection ExecutionConditionCollection { get; }

    public JobManagerConfiguration Build()
    {
        return new JobManagerConfiguration
        {
            JobDescriptions = JobDescriptionCollection.ToArray(),
            JobGroupDescriptions = JobGroupDescriptionCollection.ToArray(),
            ExecutionMiddlewareTypesCollection = ExecutionMiddlewareCollection.ToArray()
        };
    }
}
