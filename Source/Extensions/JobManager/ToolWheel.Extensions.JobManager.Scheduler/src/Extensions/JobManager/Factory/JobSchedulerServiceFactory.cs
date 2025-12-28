using System;
using Microsoft.Extensions.DependencyInjection;
using ToolWheel.Extensions.JobManager.Service;

namespace ToolWheel.Extensions.JobManager.Factory;

public class JobSchedulerServiceFactory : IJobSchedulerServiceFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IJobService _jobService;

    public JobSchedulerServiceFactory(IServiceProvider serviceProvider, IJobService jobService)
    {
        _serviceProvider = serviceProvider;
        _jobService = jobService;
    }

    public IJobSchedulerService Create()
    {
        return ActivatorUtilities.CreateInstance<JobSchedulerService>(_serviceProvider);
    }
}
