using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using ToolWheel.Extensions.JobManager.Configuration;

namespace ToolWheel.Extensions.JobManager.Service;

public class JobSchedulerService : IJobSchedulerService
{
    private readonly Dictionary<IJob, JobScheduler> _scheduledJobs = new();
    private readonly ILogger<JobSchedulerService> _logger;

    public JobSchedulerService(ILogger<JobSchedulerService> logger)
    {
        _logger = logger;
    }

    public IJobScheduler Add(IJob job, Action<JobScheduleBuilder> configure)
    {
        var builder = new JobScheduleBuilder();
        configure(builder);

        var jobSchedulerDescription = builder.Build();

        return Add(job, jobSchedulerDescription);
    }

    public IJobScheduler Add(IJob job, IJobSchedulerDescription jobSchedulerDescription)
    {
        var jobScheduler = new JobScheduler(job, jobSchedulerDescription.Entries);

        _scheduledJobs[job] = jobScheduler;

        _logger.LogInformation("Scheduled job {JobId} with {EntryCount} entries.", job.Id, jobSchedulerDescription.Entries.Count);

        return jobScheduler;
    }

    public IJobScheduler? GetJobSchedulers(IJob job)
    {
        _scheduledJobs.TryGetValue(job, out var jobScheduler);

        return jobScheduler;
    }

    public IEnumerable<IJobScheduler> GetJobSchedulers()
    {
        return _scheduledJobs.Values;
    }
}
