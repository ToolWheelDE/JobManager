using System;
using System.Collections.Generic;
using System.Linq;

namespace ToolWheel.Extensions.JobManager.Filter;

public sealed class OrFilter : IJobSchedulerFilter
{
    private readonly IReadOnlyList<IJobSchedulerFilter> _filters;

    public OrFilter(IEnumerable<IJobSchedulerFilter> filters)
    {
        var list = filters?.ToList() ?? throw new ArgumentNullException(nameof(filters));
        if (list.Count == 0)
        {
            throw new ArgumentException(nameof(filters));
        }

        _filters = list;
    }

    public bool CheckExecutable(DateTime utcNow)
    {
        foreach (var f in _filters)
        {
            if (f.CheckExecutable(utcNow))
            {
                return true;
            }
        }

        return false;
    }
}
