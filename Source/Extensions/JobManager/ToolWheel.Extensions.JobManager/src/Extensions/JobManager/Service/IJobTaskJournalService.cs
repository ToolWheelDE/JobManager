using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using ToolWheel.Extensions.JobManager;

namespace ToolWheel.Extensions.JobManager.Service;

public interface IJobTaskJournalService
{
    void AddTask(IJobTask jobTask);

    void AddTask(IJobTask jobTask, IList<IJournalEntry> list);

    void AddJournalEntry(IJobTask jobTask, string message, LogLevel level, Exception? exception = null);

    void AddJournalEntry(IJobTask jobTask, JournalEntry journalEntry);

    IEnumerable<IJournalEntry> Read(IJobTask jobTask);
}
