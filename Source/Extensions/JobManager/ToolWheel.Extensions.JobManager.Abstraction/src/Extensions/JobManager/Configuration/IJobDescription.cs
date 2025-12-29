using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace ToolWheel.Extensions.JobManager.Configuration;

public interface IJobDescription : IEquatable<JobDescription>
{
    string JobId { get; init; }

    MethodInfo Target { get; init; }

    IConfiguration? Configuration { get; init; }

    string JobName { get; init; }

    bool IsScoped { get; init; }

    int MaxExecutedJobs { get; init; }

    bool Enabled { get; init; }

    IEnumerable<string>? JobDependencyIds { get; init; }

    IEnumerable<string> Groups { get; init; }

    IDictionary<string, object> Properties { get; init; }
}
