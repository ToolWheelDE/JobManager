using System;
using Microsoft.Extensions.DependencyInjection;

namespace ToolWheel.Extensions.JobManager.Configuration;

public interface IJobManagerConfigurationBuilder
{
    IJobManagerConfigurationBuilder ConfigureServices(Action<IServiceCollection> services);

    IJobManagerConfigurationBuilder ConfigureJobs(Action<JobDescriptionCollection> configure);

    IJobManagerConfigurationBuilder ConfigureGroups(Action<JobGroupDescriptionCollection> configure);

    IJobManagerConfigurationBuilder ConfigureMiddleware(Action<ExecutionMiddlewareCollection> configure);

    IJobManagerConfigurationBuilder ConfigureExecutionCondition(Action<ExecutionConditionCollection> configure);
}
