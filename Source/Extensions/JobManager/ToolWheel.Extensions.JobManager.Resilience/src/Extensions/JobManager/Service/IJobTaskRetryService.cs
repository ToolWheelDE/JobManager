using System;
using ToolWheel.Extensions.JobManager.Strategy;

namespace ToolWheel.Extensions.JobManager.Service;

public interface IJobTaskRetryService
{
    JobRetryStrategy? Get(IJob job);

    JobRetryStrategy SetRetry(IJob job, int retryCount, TimeSpan? retryDelay = null);
}
