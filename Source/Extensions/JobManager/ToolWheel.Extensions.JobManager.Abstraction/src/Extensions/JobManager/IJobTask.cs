using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ToolWheel.Extensions.JobManager;

public interface IJobTask
{
    IJob Job { get; }

    string Id { get; }

    JobTaskStatus Status { get; }

    Task? ExecutionTask { get; }

    CancellationTokenSource? CancellationToken { get; }

    DateTime SignalTimesstamp { get; }

    DateTime? StartTimesstamp { get; }

    DateTime? CompletTimesstamp { get; }

    TimeSpan? Runtime { get; }

    IEnumerable<IJournalEntry> Journal { get; }
}
