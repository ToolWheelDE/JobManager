using System;

namespace ToolWheel.Extensions.JobManager.Filter;

public sealed class NotFilter : IJobSchedulerFilter
{
    private readonly IJobSchedulerFilter _inner;

    public NotFilter(IJobSchedulerFilter inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public bool CheckExecutable(DateTime utcNow) => !_inner.CheckExecutable(utcNow);
}
