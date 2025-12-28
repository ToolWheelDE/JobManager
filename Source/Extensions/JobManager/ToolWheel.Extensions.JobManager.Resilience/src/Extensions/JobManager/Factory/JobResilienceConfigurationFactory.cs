using System.Reflection;
using ToolWheel.Extensions.JobManager.Service;

namespace ToolWheel.Extensions.JobManager.Factory;

public class JobResilienceConfigurationFactory : IJobResilienceConfigurationFactory
{
    private readonly IJobService _jobService;
    private readonly IJobResilienceServiceFactory _jobResilienceServiceFactory;

    public JobResilienceConfigurationFactory(IJobService jobService, IJobResilienceServiceFactory jobResilienceServiceFactory)
    {
        _jobService = jobService;
        _jobResilienceServiceFactory = jobResilienceServiceFactory;
    }

    public IJobTaskRetryService CreateAndConfigureJobTaskRetryService()
    {
        var jobTaskRetryService = _jobResilienceServiceFactory.CreateJobTaskRetryService();

        foreach (var job in _jobService.Read())
        {
            var jobTaskRetryAttribute = job.Method.GetCustomAttribute<JobTaskRetryAttribute>();

            if (jobTaskRetryAttribute is not null)
            {
                jobTaskRetryService.SetRetry(job, jobTaskRetryAttribute.RetryCount, jobTaskRetryAttribute.RetryDelay);
            }
        }

        return jobTaskRetryService;
    }
}
