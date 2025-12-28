using System;
using Microsoft.Extensions.Logging;
using ToolWheel.Extensions.JobManager;

namespace ToolWheel.Extensions.JobManager.Factory;
public class JobLoggerFactory : IJobLoggerFactory
{
    private readonly Func<IJob, ILogger> _builder;

    public JobLoggerFactory(Func<IJob, ILogger> builder)
    {
        _builder = builder;
    }

    public ILogger CreateLogger(IJob job)
    {
        return _builder.Invoke(job);
    }
}
