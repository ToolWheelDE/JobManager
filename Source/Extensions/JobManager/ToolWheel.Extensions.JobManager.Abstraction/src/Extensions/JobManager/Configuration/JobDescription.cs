using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace ToolWheel.Extensions.JobManager.Configuration;

public record JobDescription(string JobId) : IJobDescription
{
    public required MethodInfo Target { get; init; }

    public IConfiguration? Configuration { get; init; } = null;

    public required string JobName { get; init; } 

    public bool IsScoped { get; init; } 

    public int MaxExecutedJobs { get; init; }

    public bool Enabled { get; init; }

    public required IEnumerable<string> JobDependencyIds { get; init; }

    public required IEnumerable<string> Groups { get; init; }

    public required IDictionary<string, object> Properties { get; init; }

    public override int GetHashCode()
    {
        return JobId.GetHashCode();
    }
}
