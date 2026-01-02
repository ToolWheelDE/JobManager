using System;

namespace ToolWheel.Extensions.JobManager.Trigger;

public sealed class FirstDayOfMonthTrigger : IJobSchedulerTrigger
{
    public bool Enabled { get; set; } = true;

    private readonly TimeZoneInfo _tz;

    public FirstDayOfMonthTrigger(DateTime? nowUtc = null, TimeZoneInfo? tz = null)
    {
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
        var year = baseLocalDate.Year;
        var month = baseLocalDate.Month;

        // Wenn baseLocalDate.Tag < 1 ist (nie), oder gleich 1 → direkt nächster Monat
        var nextMonth = new DateTime(year, month, 1).AddMonths(baseLocalDate.Day >= 1 ? 1 : 0);
        var candidate = new DateTime(nextMonth.Year, nextMonth.Month, 1);
        return LocalStartToUtc(candidate);
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
