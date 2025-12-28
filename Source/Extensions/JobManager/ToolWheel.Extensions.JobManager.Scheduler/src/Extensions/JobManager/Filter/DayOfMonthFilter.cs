using System;
using System.Collections.Generic;
using System.Linq;

namespace ToolWheel.Extensions.JobManager.Filter;

public sealed class DayOfMonthFilter : IJobSchedulerFilter
{
    private readonly SortedSet<int> _days;
    private readonly TimeZoneInfo _tz;

    public DayOfMonthFilter(IEnumerable<int> days, TimeZoneInfo? tz = null)
    {
        if (days == null) throw new ArgumentNullException(nameof(days));
        var set = new SortedSet<int>(days.Where(d => d >= 1 && d <= 31));
        if (set.Count == 0) throw new ArgumentException(nameof(days));
        _days = set;
        _tz = tz ?? TimeZoneInfo.Local;
    }

    public DayOfMonthFilter(TimeZoneInfo tz, params int[] days) : this(days, tz) { }

    public bool CheckExecutable(DateTime utcNow)
    {
        var local = TimeZoneInfo.ConvertTimeFromUtc(utcNow, _tz);
        return _days.Contains(local.Day);
    }
}
