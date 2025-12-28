using System;
using Microsoft.Extensions.Logging;

namespace ToolWheel.Extensions.JobManager;
public record JournalEntry(string Message, LogLevel Level, Exception? Exception) : IJournalEntry
{
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
