using System;

namespace ToolWheel.Extensions.JobManager;
public interface IJobTaskContextBuilder : IJobTaskContext
{
    new JobTaskStatus Status { get; set; }

    DateTime? CreateTimesstamp { get; }

    DateTime? StartTimesstamp { get; set; }

    DateTime? CompletTimesstamp { get; set; }
}
