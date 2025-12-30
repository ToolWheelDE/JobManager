using ToolWheel.Extensions.JobManager.Configuration;
using ToolWheel.Extensions.JobManager.Service;

namespace ToolWheel.Extensions.JobManager.Factory;

public class JobSchedulerCreateAndConfigurationFactory : IJobSchedulerCreateAndConfigurationFactory
{
    private readonly IJobSchedulerServiceFactory _jobSchedulerServiceFactory;
    private readonly IJobManagerConfiguration _jobManagerConfiguration;
    private readonly IJobService _jobService;

    public JobSchedulerCreateAndConfigurationFactory(IJobSchedulerServiceFactory jobSchedulerServiceFactory, IJobManagerConfiguration jobManagerConfiguration, IJobService jobService)
    {
        _jobSchedulerServiceFactory = jobSchedulerServiceFactory;
        _jobManagerConfiguration = jobManagerConfiguration;
        _jobService = jobService;
    }

    public IJobSchedulerService CreateAndConfigure()
    {
        var jobSchedulerService = _jobSchedulerServiceFactory.Create();

        foreach (var jobDescription in _jobManagerConfiguration.JobDescriptions)
        {
            if (jobDescription.TryGetProperty<JobSchedulerDescription>(out var jobSchedulerDescription))
            {
                var job = _jobService.Find(jobDescription.JobId);

                if (job is not null)
                {
                    jobSchedulerService.Add(job, jobSchedulerDescription!);
                }
            }
        }

        return jobSchedulerService;
    }
}
