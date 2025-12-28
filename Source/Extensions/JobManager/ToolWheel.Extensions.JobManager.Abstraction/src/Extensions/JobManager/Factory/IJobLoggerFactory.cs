using Microsoft.Extensions.Logging;

namespace ToolWheel.Extensions.JobManager.Factory;
public interface IJobLoggerFactory
{
    ILogger CreateLogger(IJob job);
}
