using System;
using Microsoft.Extensions.DependencyInjection;
using ToolWheel.Extensions.JobManager.Service;

namespace ToolWheel.Extensions.JobManager.Factory;

public class JobServiceFactory : IJobServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public JobServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IJobService Create()
    {
        return ActivatorUtilities.CreateInstance<JobService>(_serviceProvider);
    }
}

