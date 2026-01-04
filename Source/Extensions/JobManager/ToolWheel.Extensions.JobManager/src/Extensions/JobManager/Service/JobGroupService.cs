using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using ToolWheel.Extensions.JobManager;
using ToolWheel.Extensions.JobManager.Configuration;

namespace ToolWheel.Extensions.JobManager.Service;

public class JobGroupService : IJobGroupService
{
    private readonly Dictionary<IJobGroup, HashSet<IJob>> _jobGroupToJobs = new();
    private readonly Dictionary<IJob, HashSet<IJobGroup>> _jobToJobGroups = new();

    private readonly ILogger<JobGroupService>? _logger;

    public JobGroupService(ILogger<JobGroupService>? logger = null)
    {
        _logger = logger;
    }

    public IJobGroup? Add(IJobGroupDescription jobGroupDescription)
    {
        var jobGroup = new JobGroup(jobGroupDescription.GroupId)
        {
            Name = jobGroupDescription.GroupName,
            MaxExecutedJobs = jobGroupDescription.MaxExecutedJobs
        };

        if (_jobGroupToJobs.ContainsKey(jobGroup))
        {
            _logger?.LogWarning("Job group {JobGroupId} already exists", jobGroup.Id);
            return null;
        }

        _jobGroupToJobs.Add(jobGroup, new HashSet<IJob>());

        _logger?.LogInformation("Adding job group {JobGroupId} with name {JobGroupName}", jobGroup.Id, jobGroup.Name);
        return jobGroup;
    }

    public IJobGroup GetJobGroup(string jobGroupId)
    {
        return _jobGroupToJobs.Keys.First(jg => jg.Id == jobGroupId);
    }

    public void AddMember(IJobGroup jobGroup, IJob job)
    {
        if (GetHashSet(jobGroup).Add(job))
        {
            GetHashSet(job).Add(jobGroup);

            _logger?.LogInformation("Added job {JobId} to job group {JobGroupId}", job.Id, jobGroup.Id);
        }
    }

    public void RemoveMember(IJobGroup jobGroup, IJob job)
    {
        if (GetHashSet(jobGroup).Remove(job))
        {
            GetHashSet(job).Remove(jobGroup);

            _logger?.LogInformation("Removed job {JobId} from job group {JobGroupId}", job.Id, jobGroup.Id);
        }
    }

    public IEnumerable<IJob> GetJobGroupMembers(IJobGroup jobGroup)
    {
        return GetHashSet(jobGroup);
    }

    public IEnumerable<IJobGroup> GetJobGroupMembers(IJob job)
    {
        return GetHashSet(job);
    }

    private HashSet<IJob> GetHashSet(IJobGroup jobGroup)
    {
        if (!_jobGroupToJobs.TryGetValue(jobGroup, out var jobSet))
        {
            jobSet = new HashSet<IJob>();
            _jobGroupToJobs.Add(jobGroup, jobSet);
        }

        return jobSet;
    }

    private HashSet<IJobGroup> GetHashSet(IJob job)
    {
        if (!_jobToJobGroups.TryGetValue(job, out var jobGroupSet))
        {
            jobGroupSet = new HashSet<IJobGroup>();
            _jobToJobGroups.Add(job, jobGroupSet);
        }

        return jobGroupSet;
    }
}
