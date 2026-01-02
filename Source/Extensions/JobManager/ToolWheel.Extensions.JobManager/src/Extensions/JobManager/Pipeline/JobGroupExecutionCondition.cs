using System;
using System.Linq;
using ToolWheel.Extensions.JobManager;
using ToolWheel.Extensions.JobManager.Service;

namespace ToolWheel.Extensions.JobManager.Pipeline;
public class JobGroupExecutionCondition : IExecutionCondition
{
    private readonly IJobGroupService _jobGroupService;
    private readonly IJobTaskService _jobTaskService;

    public JobGroupExecutionCondition(IJobGroupService jobGroupService, IJobTaskService jobTaskService)
    {
        _jobGroupService = jobGroupService;
        _jobTaskService = jobTaskService;
    }

    public void CheckStatus(IJob job, ref JobStatus jobStatus)
    {
        var jobGroups = _jobGroupService.GetJobGroupMembers(job);

        foreach (var jobGroup in jobGroups)
        {
            var jobMembers = _jobGroupService.GetJobGroupMembers(jobGroup);
            var runningCount = 0;

            foreach (var jobMember in jobMembers)
            {
                var runningTasks = _jobTaskService.ReadByStatus(jobMember, JobTaskStatus.Running);
                if (runningTasks.Count() > 0)
                {
                    runningCount++;
                }
            }

            if (runningCount >= jobGroup.MaxExecutedJobs)
            {
                jobStatus = JobStatus.WaitingForGroup;
                return;
            }
        }
    }
}
