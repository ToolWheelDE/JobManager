using System.Linq;
using ToolWheel.Extensions.JobManager;
using ToolWheel.Extensions.JobManager.Service;

namespace ToolWheel.Extensions.JobManager.Pipeline;

public class JobDependenciesExecutionCondition : IExecutionCondition
{
    private readonly IJobService _jobService;
    private IJobTaskService _jobTaskService;

    public JobDependenciesExecutionCondition(IJobService jobService, IJobTaskService jobTaskService)
    {
        _jobService = jobService;
        _jobTaskService = jobTaskService;
    }

    public void CheckStatus(IJob job, ref JobStatus jobStatus)
    {
        var result = from waitForJobId in job.JobDependencyIds
                     let waitForJob = _jobService.Find(waitForJobId)
                     where _jobTaskService.ReadByStatus(waitForJob, JobTaskStatus.Running).Any()
                     select 1;

        jobStatus = result.Any() ? JobStatus.WaitingForJob : jobStatus;
    }
}
