using System;

namespace ToolWheel.Extensions.JobManager.Trigger;

public sealed class PeriodicTimerTrigger : IJobSchedulerTrigger
{
    public bool Enabled { get; set; } = true;

    private readonly TimeSpan _interval;
    private readonly DateTime _anchorUtc;

    public PeriodicTimerTrigger(TimeSpan interval, DateTime? anchorUtc = null)
    {
        if (interval <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(interval));

        _interval = interval;
        _anchorUtc = anchorUtc?.ToUniversalTime() ?? DateTime.UtcNow;
        NextExecutionTimestamp = ComputeNextDue(_anchorUtc, DateTime.UtcNow);
    }

    public DateTime NextExecutionTimestamp { get; private set; }

    public void UpdateTrigger(DateTime timestampUtc)
    {
        NextExecutionTimestamp = ComputeNextDue(_anchorUtc, timestampUtc.ToUniversalTime());
    }

    private DateTime ComputeNextDue(DateTime anchor, DateTime now)
    {
        var delta = now - anchor;
        var steps = (long)Math.Floor(delta.Ticks / (double)_interval.Ticks) + 1;
        return anchor.AddTicks(steps * _interval.Ticks);
    }
}
