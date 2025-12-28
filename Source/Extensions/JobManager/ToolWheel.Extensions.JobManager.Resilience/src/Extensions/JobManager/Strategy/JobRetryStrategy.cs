using System;

namespace ToolWheel.Extensions.JobManager.Strategy;

public record JobRetryStrategy(IJob Job)
{
    public int RetryCount { get; init; }

    public TimeSpan? RetryDelay { get; init; }
}
