using System;
using System.Diagnostics;

namespace ToolWheel.Extensions.JobManager;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public record JobGroup(string Id) : IJobGroup, IEquatable<IJobGroup>
{
    public required string Name { get; set; } = Id;

    public int MaxExecutedJobs { get; set; } = 1;

    public bool Equals(IJobGroup? other)
    {
        return Id == other?.Id;
    }
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
}


