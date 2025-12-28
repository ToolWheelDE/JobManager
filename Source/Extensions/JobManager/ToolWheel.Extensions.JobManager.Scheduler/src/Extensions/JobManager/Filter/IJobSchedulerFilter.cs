using System;

namespace ToolWheel.Extensions.JobManager.Filter;

public interface IJobSchedulerFilter
{
    bool CheckExecutable(DateTime utcNow);
}
