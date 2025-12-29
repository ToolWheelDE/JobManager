using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ToolWheel.Extensions.JobManager;
using ToolWheel.Extensions.JobManager.Configuration;
using ToolWheel.Extensions.JobManager.Factory;
using ToolWheel.Extensions.JobManager.Pipeline;

namespace ToolWheel.Extensions.JobManager.Service;

public class JobService : IJobService
{
    private readonly ILogger<JobService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly List<Job> _jobs = new List<Job>();

    public JobService(ILogger<JobService> logger, IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    public IJob? Add(IJobDescription jobDescription)
    {
        if (Find(jobDescription.JobId) is not null)
        {
            _logger.LogWarning("Job with Id '{JobId}' already exists. Skipping registration.", jobDescription.JobId);
            return null;
        }

        var job = new Job(jobDescription.JobId)
        {
            Method = jobDescription.Target,
            Name = jobDescription.JobName,
            IsScopedInstance = jobDescription.IsScoped,
            Configuration = jobDescription.Configuration ,
            MaxExecutedTasks = jobDescription.MaxExecutedJobs,
            Enabled = jobDescription.Enabled,
            JobDependencyIds = jobDescription?.JobDependencyIds?.ToList() ?? new List<string>()
        };

        var _jobLoggerFactory = _serviceProvider.GetService<IJobLoggerFactory>();
        if (_jobLoggerFactory is not null)
        {
            var jobLogger = _jobLoggerFactory?.CreateLogger(job);
            if (jobLogger is not null)
            {
                job.Logger = jobLogger;
            }
        }

        _jobs.Add(job);

        CheckCircularJobDependency();

        _logger.LogInformation("Job with Id '{JobId}' and method '{JobMethod}' registered successfully.", job.Id, job.Method);

        return job;
    }

    public IJob? Add(MethodInfo method, Action<JobDescriptionBuilder>? configure = null)
    {
        var jobDescription = JobDescriptionConfigurationUtility.CreateJobDescription(method, configure, JobIdResolver);

        return Add(jobDescription);
    }

    public IJob? Find(string jobId)
    {
        return _jobs.FirstOrDefault(j => j.Id == jobId);
    }

    public IEnumerable<IJob> Read()
    {
        return _jobs;
    }

    public void SetLastExecutionTimestamp(IJob job, DateTime timestamp)
    {
        var jobImpl = _jobs.Find(m => m.Equals(job)) ?? throw new InvalidOperationException($"Job {job} not found.");

        jobImpl.LastExecutionTimestamp = timestamp;
    }

    public JobStatus UpdateJobStatus(IJob job)
    {
        var jobStatus = JobStatus.Ready;
        var jobImpl = _jobs.Find(m => m.Equals(job)) ?? throw new InvalidOperationException($"Job {job} not found.");
        var executionConditions = _serviceProvider.GetServices<IExecutionCondition>();

        foreach (var executionCondition in executionConditions)
        {
            executionCondition.CheckStatus(job, ref jobStatus);

            if (jobStatus != JobStatus.Ready)
            {
                break;
            }
        }

        jobImpl.Status = jobStatus;

        return jobStatus;
    }

    public void AddJobDependency(IJob job, IJob waitFor)
    {
        var jobImpl = _jobs.Find(m => m.Equals(job)) ?? throw new InvalidOperationException($"Job {job} not found.");

        jobImpl.JobDependencyIds = jobImpl.JobDependencyIds.Append(waitFor.Id);

        CheckCircularJobDependency();
    }

    public void RemoveJobDependency(IJob job, IJob waitFor)
    {
        var jobImpl = _jobs.Find(m => m.Equals(job)) ?? throw new InvalidOperationException($"Job {job} not found.");

        jobImpl.JobDependencyIds = jobImpl.JobDependencyIds.Where(id => id != waitFor.Id);
    }

    private void CheckCircularJobDependency()
    {
        var removableDependencies = from job in _jobs
                                    from waitForId in job.JobDependencyIds
                                    let waitForJob = Find(waitForId)
                                    where waitForJob?.JobDependencyIds.Contains(job.Id) ?? false
                                    select new { Job = job, WaitForJobId = waitForJob.Id };

        foreach (var removableDependency in removableDependencies)
        {
            _logger.LogWarning("Circular dependency detected between Job '{JobId}' and Job '{WaitForJobId}'.", removableDependency.Job.Id, removableDependency.WaitForJobId);
        }
    }


    private string JobIdResolver(JobDescriptionIdInfo info)
    {
        var count = _jobs.Count(m => m.Method == info.Target);

        return count == 0 ? info.Target.Name : $"{info.Target.Name}_{count}";
    }
}
