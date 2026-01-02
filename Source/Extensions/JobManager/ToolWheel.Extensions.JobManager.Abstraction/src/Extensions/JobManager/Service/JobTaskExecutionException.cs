using System;
using ToolWheel.Extensions.JobManager;

namespace ToolWheel.Extensions.JobManager.Service;

[Serializable]
public class JobTaskExecutionException : Exception
{
    public JobTaskExecutionException(IJobTask jobTask)
    {
        JobTask = jobTask;
    }

    public JobTaskExecutionException(IJobTask jobTask, string? message) : base(message)
    {
        JobTask = jobTask;
    }

    public JobTaskExecutionException(IJobTask jobTask, string? message, Exception? innerException) : base(message, innerException)
    {
        JobTask = jobTask;
    }

    public IJobTask JobTask { get; }
}
