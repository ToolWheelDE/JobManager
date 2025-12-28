using System;

namespace ToolWheel.Extensions.JobManager.Filter;

public sealed class TimeWindowFilter : IJobSchedulerFilter
{
    private readonly TimeOnly _from;
    private readonly TimeOnly _to;
    private readonly TimeZoneInfo _tz;

    public TimeWindowFilter(TimeOnly from, TimeOnly to, TimeZoneInfo? tz = null)
    {
        _from = from;
        _to = to;
        _tz = tz ?? TimeZoneInfo.Local;
    }

    public bool CheckExecutable(DateTime utcNow)
    {
        var local = TimeZoneInfo.ConvertTimeFromUtc(utcNow, _tz);
        var t = new TimeOnly(local.Hour, local.Minute, local.Second);
        if (_from <= _to) return t >= _from && t <= _to;
        return t >= _from || t <= _to;
    }
}
