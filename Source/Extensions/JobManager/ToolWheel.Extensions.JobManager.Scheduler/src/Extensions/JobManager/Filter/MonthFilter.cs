using System;
using System.Collections.Generic;
using System.Linq;

namespace ToolWheel.Extensions.JobManager.Filter;

public sealed class MonthFilter : IJobSchedulerFilter
{
    private readonly HashSet<int> _months;
    private readonly TimeZoneInfo _tz;

    public MonthFilter(IEnumerable<int> months, TimeZoneInfo? tz = null)
    {
        if (months == null) throw new ArgumentNullException(nameof(months));
        var set = new HashSet<int>(months.Where(m => m >= 1 && m <= 12));
        if (set.Count == 0) throw new ArgumentException(nameof(months));
        _months = set;
        _tz = tz ?? TimeZoneInfo.Local;
    }

    public MonthFilter(TimeZoneInfo tz, params int[] months) : this(months, tz) { }

    public bool CheckExecutable(DateTime utcNow)
    {
        var local = TimeZoneInfo.ConvertTimeFromUtc(utcNow, _tz);
        return _months.Contains(local.Month);
    }
}
