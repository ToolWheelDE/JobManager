using System;

namespace ToolWheel.Extensions.JobManager;

public class JobTaskContextBuilder : IJobTaskContextBuilder
{
    public JobTaskContextBuilder(JobTask jobTask)
    {
        JobTask = jobTask;
    }

    public JobTaskContext Build()
    {
        return new JobTaskContext(JobTask)
        {
            Journal = Journal
        };
    }

    public IJobTask JobTask { get; }

    public IJobLogger? Journal { get; set; }
}
