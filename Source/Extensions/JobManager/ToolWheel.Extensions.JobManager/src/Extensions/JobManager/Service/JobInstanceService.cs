using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace ToolWheel.Extensions.JobManager.Service;
public class JobInstanceService : IJobInstanceService
{
    private readonly Dictionary<IJob, object> _jobInstances = new();
    private readonly IServiceProvider _serviceProvider;

    public JobInstanceService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public object? GetJobInstance(IJob job)
    {
        if (job.Method.IsStatic)
        {
            return null;
        }

        if (job.IsScopedInstance)
        {
            return ActivatorUtilities.CreateInstance(_serviceProvider, job.Method.DeclaringType!);
        }

        if (!_jobInstances.TryGetValue(job, out var instance))
        {
            instance = ActivatorUtilities.CreateInstance(_serviceProvider, job.Method.DeclaringType!);
            _jobInstances[job] = instance;
        }

        return instance;
    }

    public int Count()
    {
        return _jobInstances.Count;
    }
}
