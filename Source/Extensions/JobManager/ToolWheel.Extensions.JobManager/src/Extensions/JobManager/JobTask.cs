using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ToolWheel.Extensions.JobManager;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public record JobTask(IJob Job, string Id) : IJobTask
{
    public JobTaskStatus Status { get; internal set; } = JobTaskStatus.Pending;

    public Task? ExecutionTask { get; internal set; } = null;

    public CancellationTokenSource? CancellationToken { get; internal set; }

    public DateTime SignalTimesstamp { get; } = DateTime.UtcNow;

    public DateTime? StartTimesstamp { get; internal set; }

    public DateTime? CompletTimesstamp { get; internal set; }

    public TimeSpan? Runtime { get => (CompletTimesstamp ?? DateTime.UtcNow) - StartTimesstamp; }

    public IEnumerable<IJournalEntry> Journal { get; internal set; } = Array.Empty<IJournalEntry>();

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }

    public bool Equals(IJob? other)
    {
        return other?.Id == Id;
    }

    override public string ToString()
    {
        return $"JobTask: {Job.Name}, Id: {Id}, Status: {Status}";
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}
