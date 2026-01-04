using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ToolWheel.Extensions.JobManager;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public record Job(string Id) : IJob, IEquatable<IJob>
{
    public required MethodInfo Method { get; init; }

    public required object? Target { get; init; }

    public required string Name { get; set; }

    public required bool IsScopedInstance { get; init; }

    public IConfiguration? Configuration { get; init; }

    public int MaxExecutedTasks { get; set; } = 1;

    public DateTime? LastExecutionTimestamp { get; internal set; }

    public ILogger? Logger { get; set; }

    public bool Enabled { get; set; } = true;

    public JobStatus Status { get; internal set; } = JobStatus.Ready;

    public IEnumerable<string> JobDependencyIds { get; set; } = [];

    public override string ToString()
    {
        return $"Job: {Name}, Id: {Id}";
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }

    public bool Equals(IJob? other)
    {
        return other?.Id == Id;
    }
}
