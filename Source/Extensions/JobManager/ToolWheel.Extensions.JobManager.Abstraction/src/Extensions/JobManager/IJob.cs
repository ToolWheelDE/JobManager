using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ToolWheel.Extensions.JobManager;

public interface IJob
{
    string Id { get; init; }

    MethodInfo Method { get; init; }

    bool IsScopedInstance { get; init; }

    JobStatus Status { get; }

    bool Enabled { get; set; }

    IConfiguration Configuration { get; init; }

    string Name { get; set; }

    int MaxExecutedTasks { get; set; }

    DateTime? LastExecutionTimestamp { get; }

    IEnumerable<string> JobDependencyIds { get; }

    ILogger? Logger { get; set; }
}
