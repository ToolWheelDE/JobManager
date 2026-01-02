using System;
using Microsoft.Extensions.Logging;

namespace ToolWheel.Extensions.JobManager;

public interface IJournalEntry
{
    DateTime Timestamp { get; }

    string Message { get; }

    LogLevel Level { get; }

    Exception? Exception { get; }
}
