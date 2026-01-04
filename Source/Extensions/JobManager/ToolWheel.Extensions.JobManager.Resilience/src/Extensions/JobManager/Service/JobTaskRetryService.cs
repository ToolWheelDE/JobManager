using System;
using System.Collections.Generic;
using ToolWheel.Extensions.JobManager.Strategy;

namespace ToolWheel.Extensions.JobManager.Service;

public class JobTaskRetryService : IJobTaskRetryService
{
    private readonly Dictionary<IJob, JobRetryStrategy> _retryStrategies = new Dictionary<IJob, JobRetryStrategy>();

    public JobTaskRetryService()
    { }

    public JobRetryStrategy? Get(IJob job)
    {
        _retryStrategies.TryGetValue(job, out var strategy);

        return strategy;
    }

    public JobRetryStrategy SetRetry(IJob job, int retryCount, TimeSpan? retryDelay = null)
    {
        if (job == null) throw new ArgumentNullException(nameof(job));
        if (retryCount < 0) throw new ArgumentOutOfRangeException(nameof(retryCount), "Retry count must be non-negative.");

        var strategy = new JobRetryStrategy(job)
        {
            RetryCount = retryCount,
            RetryDelay = retryDelay
        };

        _retryStrategies[job] = strategy;

        return strategy;
    }
}
