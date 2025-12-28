using ToolWheel.Extensions.JobManager.Service;

namespace ToolWheel.Extensions.JobManager.Factory;

public interface IJobGroupServiceFactory
{
    IJobGroupService Create();
}
