using System;
using System.Collections.Generic;

namespace ToolWheel.Extensions.JobManager.Filter;

public sealed class WeekdayFilter : IJobSchedulerFilter
{
    private readonly HashSet<DayOfWeek> _allowed;
    private readonly TimeZoneInfo _tz;

    public WeekdayFilter(IEnumerable<DayOfWeek> allowed, TimeZoneInfo? tz = null)
    {
        if (allowed == null) throw new ArgumentNullException(nameof(allowed));
        var set = new HashSet<DayOfWeek>(allowed);
        if (set.Count == 0) throw new ArgumentException(nameof(allowed));
        _allowed = set;
        _tz = tz ?? TimeZoneInfo.Local;
    }

    public WeekdayFilter(TimeZoneInfo tz, params DayOfWeek[] allowed) : this(allowed, tz)
    { }

    public bool CheckExecutable(DateTime utcNow)
    {
        var local = TimeZoneInfo.ConvertTimeFromUtc(utcNow, _tz);
        return _allowed.Contains(local.DayOfWeek);
    }
}
