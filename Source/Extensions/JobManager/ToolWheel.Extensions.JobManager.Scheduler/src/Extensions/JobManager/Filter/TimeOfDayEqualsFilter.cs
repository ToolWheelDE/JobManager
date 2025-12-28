using System;

namespace ToolWheel.Extensions.JobManager.Filter;

public sealed class TimeOfDayEqualsFilter : IJobSchedulerFilter
{
    private readonly TimeOnly _time;
    private readonly TimeZoneInfo _tz;

    public TimeOfDayEqualsFilter(TimeOnly time, TimeZoneInfo? tz = null)
    {
        _time = time;
        _tz = tz ?? TimeZoneInfo.Local;
    }

    public bool CheckExecutable(DateTime utcNow)
    {
        var local = TimeZoneInfo.ConvertTimeFromUtc(utcNow, _tz);
        return local.Hour == _time.Hour && local.Minute == _time.Minute && local.Second == _time.Second;
    }
}
