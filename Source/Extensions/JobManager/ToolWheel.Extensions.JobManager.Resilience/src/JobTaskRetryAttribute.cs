using System;

namespace ToolWheel;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class JobTaskRetryAttribute : Attribute
{
    public JobTaskRetryAttribute(int retryCount)
    {
        RetryCount = retryCount;
        RetryDelay = null;
    }

    public JobTaskRetryAttribute(int retryCount, TimeSpan retryDelay)
    {
        RetryCount = retryCount;
        RetryDelay = retryDelay;
    }

    public int RetryCount { get; }

    public TimeSpan? RetryDelay { get; }
}
