namespace ToolWheel.Extensions.JobManager;

public interface IJobTaskContextBuilder
{
    IJobTask JobTask { get; }

    IJobLogger? Journal { get; set; }
}
