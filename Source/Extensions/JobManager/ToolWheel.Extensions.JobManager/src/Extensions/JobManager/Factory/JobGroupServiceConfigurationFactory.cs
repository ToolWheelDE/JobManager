using System;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using ToolWheel.Extensions.JobManager.Service;
using ToolWheel.Extensions.JobManager.Configuration;

namespace ToolWheel.Extensions.JobManager.Factory;

public class JobGroupServiceConfigurationFactory : IJobGroupServiceConfigurationFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IJobGroupServiceFactory _jobGroupServiceFactory;
    private readonly IJobManagerConfiguration _jobManagerConfiguration;

    public JobGroupServiceConfigurationFactory(IServiceProvider serviceProvider, IJobGroupServiceFactory jobGroupServiceFactory, IJobManagerConfiguration jobManagerConfiguration)
    {
        _serviceProvider = serviceProvider;
        _jobGroupServiceFactory = jobGroupServiceFactory;
        _jobManagerConfiguration = jobManagerConfiguration;
    }

    public IJobGroupService CreateAndConfigure()
    {
        var jobGroupService = _jobGroupServiceFactory.Create();
        var jobService = _serviceProvider.GetRequiredService<IJobService>();

        foreach (var jobGroupDescription in _jobManagerConfiguration.JobGroupDescriptions)
        {
            var jobGroup = jobGroupService.Add(jobGroupDescription);

            if (jobGroup is null)
            {
                continue;
            }

            var jobs = from jobDescription in _jobManagerConfiguration.JobDescriptions
                       where jobDescription.Groups.Contains(jobGroupDescription.GroupId)
                       let job = jobService.Find(jobDescription.JobId)
                       where job is not null
                       select job;

            foreach (var job in jobs)
            {
                jobGroupService.AddMember(jobGroup, job);
            }
        }

        return jobGroupService;
    }
}
