namespace ToolWheel.Extensions.JobManager;
public interface IJobTaskContext
{
    IJobTask JobTask { get; init; }

    IJobLogger? Journal { get; init; }
}
