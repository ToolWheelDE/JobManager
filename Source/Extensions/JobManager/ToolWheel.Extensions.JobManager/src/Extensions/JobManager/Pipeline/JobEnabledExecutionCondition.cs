namespace ToolWheel.Extensions.JobManager.Pipeline;

public class JobEnabledExecutionCondition : IExecutionCondition
{
    public readonly static JobStatus JobStatusNotReadyDisabled = new JobStatus(JobStatusEnum.NotReady, "Disabled");

    public void CheckStatus(IJob job, ref JobStatus status)
    {
        if (!job.Enabled)
        {
            status = JobStatusNotReadyDisabled;
        }
    }
}
