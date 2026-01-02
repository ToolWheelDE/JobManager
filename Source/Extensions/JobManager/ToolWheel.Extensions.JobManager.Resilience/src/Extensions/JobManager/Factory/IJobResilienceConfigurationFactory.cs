using ToolWheel.Extensions.JobManager.Service;

namespace ToolWheel.Extensions.JobManager.Factory;

public interface IJobResilienceConfigurationFactory
{
    IJobTaskRetryService CreateAndConfigureJobTaskRetryService();
}
