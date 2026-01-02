using System;

namespace ToolWheel.Extensions.JobManager.Configuration;
public interface IJobManagerConfiguration
{
    Type[] ExecutionMiddlewareTypesCollection { get; init; }

    IJobDescription[] JobDescriptions { get; init; }

    IJobGroupDescription[] JobGroupDescriptions { get; init; }
}
