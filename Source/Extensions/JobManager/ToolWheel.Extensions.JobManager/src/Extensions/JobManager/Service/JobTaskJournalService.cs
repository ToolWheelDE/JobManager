using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ToolWheel.Extensions.JobManager;

namespace ToolWheel.Extensions.JobManager.Service;
public class JobTaskJournalService : IJobTaskJournalService
{
    private readonly Dictionary<IJobTask, IList<IJournalEntry>> _journalEntries = new();

    public JobTaskJournalService()
    { }

    public void AddJournalEntry(IJobTask jobTask, JournalEntry journalEntry)
    {
        _journalEntries[jobTask].Add(journalEntry);
    }

    public void AddJournalEntry(IJobTask jobTask, string message, LogLevel level, Exception? exception = null)
    {
        AddJournalEntry(jobTask, new JournalEntry
        (
            message,
            level,
            exception
        ));
    }

    public void AddTask(IJobTask jobTask)
    {
        _journalEntries[jobTask] = new List<IJournalEntry>();
    }

    public void AddTask(IJobTask jobTask, IList<IJournalEntry> list)
    {
        _journalEntries[jobTask] = list;
    }

    public IEnumerable<IJournalEntry> Read(IJobTask jobTask)
    {
        return _journalEntries[jobTask];
    }
}
