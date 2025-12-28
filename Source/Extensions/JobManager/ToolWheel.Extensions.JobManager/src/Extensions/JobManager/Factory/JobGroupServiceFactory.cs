using System;
using Microsoft.Extensions.DependencyInjection;
using ToolWheel.Extensions.JobManager.Service;

namespace ToolWheel.Extensions.JobManager.Factory;

public class JobGroupServiceFactory : IJobGroupServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public JobGroupServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IJobGroupService Create()
    {
        return ActivatorUtilities.CreateInstance<JobGroupService>(_serviceProvider);
    }
}
