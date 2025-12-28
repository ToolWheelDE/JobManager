using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace ToolWheel.Extensions.JobManager;
public class JournalLogger : IJobLogger
{
    private readonly ILogger? _logger;
    private readonly IList<IJournalEntry> _journalList;

    public JournalLogger(ILogger? logger, IList<IJournalEntry> journalList)
    {
        _logger = logger;
        _journalList = journalList;
    }

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull
    {
        return _logger?.BeginScope(state);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _logger?.IsEnabled(logLevel) ?? false;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _journalList.Add(new JournalEntry
        (
            formatter(state, exception),
            logLevel,
            exception
        ));

        _logger?.Log(logLevel, eventId, state, exception, formatter);
    }
}
