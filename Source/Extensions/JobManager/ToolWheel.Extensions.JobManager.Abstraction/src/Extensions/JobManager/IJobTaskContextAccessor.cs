namespace ToolWheel.Extensions.JobManager;
public interface IJobTaskContextAccessor
{
    IJobTaskContext? JobTaskContext { get; }
}
