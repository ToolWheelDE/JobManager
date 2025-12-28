namespace ToolWheel.Extensions.JobManager.Configuration;

public interface IJobGroupDescription
{
    string GroupId { get; init; }

    string GroupName { get; init; }

    int MaxExecutedJobs { get; set; }
}
