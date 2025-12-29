using ToolWheel.Extensions.JobManager.Configuration;
using ToolWheel.Extensions.JobManager.Service;

namespace ToolWheel.Extensions.JobManager.Factory;

public class JobServiceConfigurationFactory : IJobServiceConfigurationFactory
{
    private readonly IJobServiceFactory _jobServiceFactory;
    private readonly IJobManagerConfiguration _jobManagerConfiguration;

    public JobServiceConfigurationFactory(IJobServiceFactory jobServiceFactory, IJobManagerConfiguration jobManagerConfiguration)
    {
        _jobServiceFactory = jobServiceFactory;
        _jobManagerConfiguration = jobManagerConfiguration;
    }

    public IJobService CreateAndConfigure()
    {
        var jobService = _jobServiceFactory.Create();

        foreach (var jobDescription in _jobManagerConfiguration.JobDescriptions)
        {
            jobService.Add(jobDescription);
        }

        return jobService;
    }
}

