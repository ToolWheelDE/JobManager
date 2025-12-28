using ToolWheel.Extensions.JobManager.Service;

namespace ToolWheel.Extensions.JobManager.Factory;

public interface IJobResilienceServiceFactory
{
    IJobTaskRetryService CreateJobTaskRetryService();
}
