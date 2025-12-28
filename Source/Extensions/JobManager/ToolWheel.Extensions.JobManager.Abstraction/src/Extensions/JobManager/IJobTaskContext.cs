namespace ToolWheel.Extensions.JobManager;
public interface IJobTaskContext
{
    IJobTask JobTask { get; }

    JobTaskStatus Status { get; }

    IJobLogger? Journal { get; }
}
