using Microsoft.Extensions.Configuration;
using ToolWheel.Extensions.JobManager.Configuration;
using ToolWheel.Extensions.JobManager.Service;

namespace ToolWheel.Extensions.JobManager.Factory;

public class JobServiceConfigurationFactory : IJobServiceConfigurationFactory
{
        private readonly IJobServiceFactory _jobServiceFactory;
    private readonly IJobManagerConfiguration _jobManagerConfiguration;
    private readonly IConfiguration _configuration;

    public JobServiceConfigurationFactory(IJobServiceFactory jobServiceFactory, IJobManagerConfiguration jobManagerConfiguration, IConfiguration configuration)
    {
        _jobServiceFactory = jobServiceFactory;
        _jobManagerConfiguration = jobManagerConfiguration;
        _configuration = configuration;
    }

    public IJobService CreateAndConfigure()
    {
        var jobService = _jobServiceFactory.Create();

        foreach (var jobDescription in _jobManagerConfiguration.JobDescriptions)
        {
            jobDescription.Configuration = _configuration.GetSection("Jobs:" + jobDescription.JobId);

            jobService.Add(jobDescription);
        }

        return jobService;
    }
}

