namespace ToolWheel.Extensions.JobManager.Configuration;
public record JobGroupDescription(string GroupId) : IJobGroupDescription
{
    public required string GroupName { get; init; } = GroupId;

    public int MaxExecutedJobs { get; set; } = 1;
}
