using System;

namespace ToolWheel.Extensions.JobManager.Filter;

public sealed class AllowAllFilter : IJobSchedulerFilter
{
    public bool CheckExecutable(DateTime utcNow)
    {
        return true;
    }
}
