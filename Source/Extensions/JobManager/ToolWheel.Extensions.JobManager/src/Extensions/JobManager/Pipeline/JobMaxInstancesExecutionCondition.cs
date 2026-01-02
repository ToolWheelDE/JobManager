using System;
using System.Linq;
using ToolWheel.Extensions.JobManager;
using ToolWheel.Extensions.JobManager.Service;

namespace ToolWheel.Extensions.JobManager.Pipeline;

public class JobMaxInstancesExecutionCondition : IExecutionCondition
{
    private readonly IJobTaskService _jobTaskService;
    public readonly static JobStatus JobStatusNotReadyByMaxInstances = new JobStatus(JobStatusEnum.NotReady, "MaxInstaces");

    public JobMaxInstancesExecutionCondition(IJobTaskService jobTaskService)
    {
        _jobTaskService = jobTaskService;
    }

    public void CheckStatus(IJob job, ref JobStatus status)
    {
        if (job.MaxExecutedTasks == 0)
        {
            return;
        }

        var tasks = _jobTaskService.ReadByStatus(job, JobTaskStatus.Running).Count();

        if (tasks >= job.MaxExecutedTasks)
        {
            status = JobStatusNotReadyByMaxInstances;
        }
    }
}
