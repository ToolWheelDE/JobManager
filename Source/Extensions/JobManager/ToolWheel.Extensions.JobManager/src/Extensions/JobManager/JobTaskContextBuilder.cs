using System;

namespace ToolWheel.Extensions.JobManager;
public class JobTaskContextBuilder : IJobTaskContextBuilder
{
    private readonly JobTask _jobTask;
    private JobTaskContext _jobTaskContext;

    public JobTaskContextBuilder(JobTask jobTask, JobTaskContext jobTaskContext)
    {
        _jobTask = jobTask ?? throw new ArgumentNullException(nameof(jobTask));
        _jobTaskContext = jobTaskContext ?? throw new ArgumentNullException(nameof(jobTaskContext));
    }

    public IJobTask JobTask { get => _jobTask; }

    public JobTaskStatus Status { get => _jobTask.Status; set => _jobTask.Status = value; }

    public DateTime? CreateTimesstamp { get => _jobTask.SignalTimesstamp; }

    public DateTime? StartTimesstamp { get => _jobTask.StartTimesstamp; set => _jobTask.StartTimesstamp = value; }

    public DateTime? CompletTimesstamp { get => _jobTask.CompletTimesstamp; set => _jobTask.CompletTimesstamp = value; }

    public IJobLogger? Journal { get => _jobTaskContext.Journal; }
}
