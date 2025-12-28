using ToolWheel.Extensions.JobManager;

namespace ToolWheel.Extensions.JobManager.Service;

public interface IJobTaskExecutionService
{
    IJobTask? Execute(IJob job);
}
