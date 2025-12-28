using System;

namespace ToolWheel.Extensions.JobManager.Trigger;

public sealed class LastDayOfMonthTrigger : IJobSchedulerTrigger
{
    public bool Enabled { get; set; } = true;

    private readonly TimeZoneInfo _tz;

    public LastDayOfMonthTrigger(DateTime? nowUtc = null, TimeZoneInfo? tz = null)
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
        // Kandidat in aktuellem Monat?
        var year = baseLocalDate.Year;
        var month = baseLocalDate.Month;

        var lastThisMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));

        // STRICTLY GREATER als baseLocalDate
        if (baseLocalDate < lastThisMonth)
        {
            return LocalStartToUtc(lastThisMonth);
        }

        // Sonst nächster Monat, letzter Tag
        var nextMonth = new DateTime(year, month, 1).AddMonths(1);
        var lastNextMonth = new DateTime(nextMonth.Year, nextMonth.Month, DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month));
        return LocalStartToUtc(lastNextMonth);
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
