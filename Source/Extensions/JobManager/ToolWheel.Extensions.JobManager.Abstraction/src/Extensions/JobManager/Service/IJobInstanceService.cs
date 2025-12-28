using ToolWheel.Extensions.JobManager;

namespace ToolWheel.Extensions.JobManager.Service;

public interface IJobInstanceService
{
    object? GetJobInstance(IJob job);

    int Count();
}
