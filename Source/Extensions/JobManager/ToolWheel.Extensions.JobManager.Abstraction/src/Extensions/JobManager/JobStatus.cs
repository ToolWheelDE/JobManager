namespace ToolWheel.Extensions.JobManager;

public record JobStatus(JobStatusEnum Status, string Text)
{
    public static JobStatus Ready = new JobStatus(JobStatusEnum.Ready, "Ready");
    public static JobStatus NotReady = new JobStatus(JobStatusEnum.NotReady, "NotReady");
    public static JobStatus WaitingForJob = new JobStatus(JobStatusEnum.NotReady, "WaitingForJob");
    public static JobStatus WaitingForGroup = new JobStatus(JobStatusEnum.NotReady, "WaitingForGroup");
}


