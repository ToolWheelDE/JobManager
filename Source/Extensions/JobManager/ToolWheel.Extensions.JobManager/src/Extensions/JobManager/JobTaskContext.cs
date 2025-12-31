using System.Diagnostics;

namespace ToolWheel.Extensions.JobManager;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public record JobTaskContext(IJobTask JobTask) : IJobTaskContext
{
    public IJobLogger? Journal { get; init; }

    override public string ToString()
    {
        return $"JobTaskContext: {JobTask.Job.Name}, Id: {JobTask.Id}";
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}
