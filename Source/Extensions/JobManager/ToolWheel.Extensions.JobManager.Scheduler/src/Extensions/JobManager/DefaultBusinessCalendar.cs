using System;
using System.Collections.Generic;
using System.Linq;

namespace ToolWheel.Extensions.JobManager;

public sealed class DefaultBusinessCalendar : IBusinessCalendar
{
    private readonly HashSet<DateTime> _holidays; // Datumsanteil (Date) in lokaler TZ erwartet

    public DefaultBusinessCalendar(IEnumerable<DateTime>? holidays = null)
    {
        _holidays = holidays != null
            ? new HashSet<DateTime>(holidays.Select(d => d.Date))
            : new HashSet<DateTime>();
    }

    public bool IsBusinessDay(DateTime localDate)
    {
        var d = localDate.Date;
        if (_holidays.Contains(d)) return false;

        var dow = d.DayOfWeek;
        return dow != DayOfWeek.Saturday && dow != DayOfWeek.Sunday;
    }
}
