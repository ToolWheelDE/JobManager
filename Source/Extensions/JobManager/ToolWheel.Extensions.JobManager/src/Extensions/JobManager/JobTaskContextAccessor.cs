namespace ToolWheel.Extensions.JobManager;

public class JobTaskContextAccessor : IJobTaskContextAccessor
{
    public IJobTaskContext? JobTaskContext { get; internal set; } = null;
}
