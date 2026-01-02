using System;

namespace ToolWheel.Extensions.JobManager.Configuration;

public class JobManagerConfiguration : IJobManagerConfiguration
{
    public JobManagerConfiguration()
    { }

    public IJobDescription[] JobDescriptions { get; init; } = Array.Empty<IJobDescription>();

    public IJobGroupDescription[] JobGroupDescriptions { get; init; } = Array.Empty<IJobGroupDescription>();

    public Type[] ExecutionMiddlewareTypesCollection { get; init; } = Array.Empty<Type>();
}
