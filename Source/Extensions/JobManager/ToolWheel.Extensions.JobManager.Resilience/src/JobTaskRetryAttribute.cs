using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
