using System;

namespace ToolWheel.Extensions.JobManager.Filter;

public sealed class NthWeekdayFilter : IJobSchedulerFilter
{
    private readonly int _n;
    private readonly DayOfWeek _dayOfWeek;
    private readonly TimeZoneInfo _tz;

    public NthWeekdayFilter(int n, DayOfWeek dayOfWeek, TimeZoneInfo? tz = null)
    {
        if (n < 1 || n > 5) throw new ArgumentOutOfRangeException(nameof(n));
        _n = n;
        _dayOfWeek = dayOfWeek;
        _tz = tz ?? TimeZoneInfo.Local;
    }

    public bool CheckExecutable(DateTime utcNow)
    {
        var local = TimeZoneInfo.ConvertTimeFromUtc(utcNow, _tz).Date;
        if (local.DayOfWeek != _dayOfWeek) return false;
        var first = new DateTime(local.Year, local.Month, 1);
        var offset = ((int)_dayOfWeek - (int)first.DayOfWeek + 7) % 7;
        var firstOcc = first.AddDays(offset);
        var nth = firstOcc.AddDays((_n - 1) * 7);
        return nth.Month == local.Month && local == nth;
    }
}
