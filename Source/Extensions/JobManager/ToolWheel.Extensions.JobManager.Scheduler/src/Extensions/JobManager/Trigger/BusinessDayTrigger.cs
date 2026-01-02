using System;
using ToolWheel.Extensions.JobManager;

namespace ToolWheel.Extensions.JobManager.Trigger;

public sealed class BusinessDayTrigger : IJobSchedulerTrigger
{
    public bool Enabled { get; set; } = true;

    private readonly TimeZoneInfo _tz;
    private readonly IBusinessCalendar _calendar;

    public BusinessDayTrigger(IBusinessCalendar calendar, DateTime? nowUtc = null, TimeZoneInfo? tz = null)
    {
        _calendar = calendar ?? throw new ArgumentNullException(nameof(calendar));
        _tz = tz ?? TimeZoneInfo.Local;

        var startUtc = nowUtc ?? DateTime.UtcNow;
        NextExecutionTimestamp = CalcNextDueAfterLocalDate(LocalDate(startUtc).AddDays(-1));
    }

    public DateTime NextExecutionTimestamp { get; private set; }

    public bool CheckExecutable(DateTime timestampUtc)
    {
        return Enabled && timestampUtc >= NextExecutionTimestamp;
    }

    public void UpdateTrigger(DateTime timestampUtc)
    {
        var lastLocal = LocalDate(NextExecutionTimestamp);
        NextExecutionTimestamp = CalcNextDueAfterLocalDate(lastLocal);
    }

    private DateTime CalcNextDueAfterLocalDate(DateTime baseLocalDate)
    {
        // Finde den nächsten Business-Tag STRICTLY AFTER baseLocalDate
        var d = baseLocalDate.AddDays(1);
        while (!_calendar.IsBusinessDay(d))
        {
            d = d.AddDays(1);
        }
        return LocalStartToUtc(d);
    }

    private DateTime LocalDate(DateTime utc)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(utc, _tz).Date;
    }

    private DateTime LocalStartToUtc(DateTime localDate)
    {
        var d = new DateTime(localDate.Year, localDate.Month, localDate.Day, 0, 0, 0, DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(d, _tz);
    }
}
