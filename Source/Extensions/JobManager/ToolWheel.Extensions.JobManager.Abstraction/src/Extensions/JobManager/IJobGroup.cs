namespace ToolWheel.Extensions.JobManager;

public interface IJobGroup
{
    string Id { get; init; }

    string Name { get; set; }

    int MaxExecutedJobs { get; set; }
}
