using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolWheel.Extensions.JobManager.Service;
using Microsoft.Extensions.DependencyInjection;

namespace ToolWheel.Extensions.JobManager.Factory;

public class JobResilienceServiceFactory : IJobResilienceServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public JobResilienceServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IJobTaskRetryService CreateJobTaskRetryService()
    {
        return ActivatorUtilities.CreateInstance<JobTaskRetryService>(_serviceProvider);
    }
}
