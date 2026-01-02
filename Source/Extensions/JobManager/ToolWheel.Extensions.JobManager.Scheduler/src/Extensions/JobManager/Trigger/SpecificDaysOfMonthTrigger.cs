using System;
using System.Collections.Generic;
using System.Linq;

namespace ToolWheel.Extensions.JobManager.Trigger;

public sealed class SpecificDaysOfMonthTrigger : IJobSchedulerTrigger
{
    public bool Enabled { get; set; } = true;

    private readonly TimeZoneInfo _tz;
    private readonly SortedSet<int> _days; // 1..31

    public SpecificDaysOfMonthTrigger(IEnumerable<int> days, DateTime? nowUtc = null, TimeZoneInfo? tz = null)
    {
        var normalized = new SortedSet<int>(days.Where(d => d >= 1 && d <= 31));
        if (normalized.Count == 0)
        {
            throw new ArgumentException("Mindestens ein Tag im Bereich 1..31 erforderlich.", nameof(days));
        }

        _days = normalized;
        _tz = tz ?? TimeZoneInfo.Local;

        var startUtc = nowUtc ?? DateTime.UtcNow;
        NextExecutionTimestamp = CalcNextDueAfterLocalDate(LocalDate(startUtc).AddDays(-1)); // erlaubt „heute“ als erste Fälligkeit
    }

    public DateTime NextExecutionTimestamp { get; private set; }

    public bool CheckExecutable(DateTime timestampUtc) => Enabled && timestampUtc >= NextExecutionTimestamp;

    public void UpdateTrigger(DateTime timestampUtc)
    {
        // Springe auf den nächsten passenden lokalen Tag NACH dem bereits fälligen Tag.
        var lastLocal = LocalDate(NextExecutionTimestamp);
        NextExecutionTimestamp = CalcNextDueAfterLocalDate(lastLocal);
    }

    private DateTime CalcNextDueAfterLocalDate(DateTime baseLocalDate)
    {
        // Finde den kleinsten passenden Tag STRICTLY GREATER als baseLocalDate.Day
        var year = baseLocalDate.Year;
        var month = baseLocalDate.Month;

        // Versuche zuerst im laufenden Monat nach baseLocalDate
        var daysInMonth = DateTime.DaysInMonth(year, month);
        var validThisMonth = _days.Where(d => d <= daysInMonth).ToList();

        int? nextDay = validThisMonth.FirstOrDefault(d => d > baseLocalDate.Day);
        if (nextDay.HasValue && nextDay.Value != 0)
        {
            var nextLocal = new DateTime(year, month, nextDay.Value);
            return LocalStartToUtc(nextLocal);
        }

        // Sonst nächster Monat
        var nextMonthLocal = new DateTime(year, month, 1).AddMonths(1);
        daysInMonth = DateTime.DaysInMonth(nextMonthLocal.Year, nextMonthLocal.Month);
        var validNextMonth = _days.Where(d => d <= daysInMonth).ToList();
        var chosen = validNextMonth.First(); // SortedSet garantiert Ordnung; es existiert min. 1
        var candidate = new DateTime(nextMonthLocal.Year, nextMonthLocal.Month, chosen);
        return LocalStartToUtc(candidate);
    }

    private DateTime LocalDate(DateTime utc)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(utc, _tz).Date;
    }

    private DateTime LocalStartToUtc(DateTime localDate)
    {
        // 00:00 lokaler Tagesbeginn → UTC
        var unspecifiedLocalMidnight = new DateTime(localDate.Year, localDate.Month, localDate.Day, 0, 0, 0, DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(unspecifiedLocalMidnight, _tz);
    }
}
