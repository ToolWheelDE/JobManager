using System.Diagnostics;

namespace ToolWheel.Extensions.JobManager;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class JobTaskContext : IJobTaskContext
{
    private readonly IJobTask _jobTask;

    public JobTaskContext(IJobTask jobTask)
    {
        _jobTask = jobTask;
    }

    public IJobTask JobTask { get => _jobTask; }

    public JobTaskStatus Status { get => _jobTask.Status; }

    public IJobLogger? Journal { get; internal set; }

    override public string ToString()
    {
        return $"JobTaskContext: {_jobTask.Job.Name}, Id: {_jobTask.Id}, Status: {Status}";
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}
