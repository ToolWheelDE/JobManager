namespace ToolWheel.Extensions.JobManager.Pipeline;
public interface IExecutionCondition
{
    void CheckStatus(IJob job, ref JobStatus jobStatus);
}
