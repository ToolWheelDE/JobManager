using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace ToolWheel.Extensions.JobManager.Configuration;

public class JobDescriptionBuilder
{
    private readonly MethodInfo _target;
    private string? _id;
    private string? _name;
    private int? _maxExecutedTasks;
    private bool? _isScoped;
    private bool? _enabled;
    private readonly HashSet<string> _jobDependencyIds = new HashSet<string>();
    private readonly HashSet<string> _jobGroupIds = new HashSet<string>();
    private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();

    public JobDescriptionBuilder(MethodInfo target, string id)
    {
        _target = target;
        _id = id;
    }

    public JobDescription Build()
    {
        return new JobDescription(_id)
        {
            Target = _target,
            JobName = _name ?? _target.Name,
            MaxExecutedJobs = _maxExecutedTasks ?? 0,
            IsScoped = _isScoped ?? true,
            Enabled = _enabled ?? true,
            JobDependencyIds = _jobDependencyIds.ToArray(),
            Groups = _jobGroupIds.ToArray(),
            Properties = new Dictionary<string, object>(_properties)
        };
    }

    public JobDescriptionBuilder Id(string id)
    {
        _id = id;

        return this;
    }

    public JobDescriptionBuilder Name(string name)
    {
        _name = name;
        return this;
    }

    public JobDescriptionBuilder MaxExecutedTasks(int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Max executed tasks must be non-negative.");
        }

        _maxExecutedTasks = count;
        return this;
    }

    public JobDescriptionBuilder IsScoped()
    {
        _isScoped = true;
        return this;
    }

    public JobDescriptionBuilder IsSingleton()
    {
        _isScoped = false;
        return this;
    }

    public JobDescriptionBuilder Enabled()
    {
        _enabled = true;
        return this;
    }

    public JobDescriptionBuilder Disabled()
    {
        _enabled = false;
        return this;
    }

    public JobDescriptionBuilder AddJobDependencyId(string jobId)
    {
        _jobDependencyIds.Add(jobId);
        return this;
    }

    public JobDescriptionBuilder AddJobDependencyId(IEnumerable<string> jobIds)
    {
        foreach (var jobId in jobIds)
        {
            _jobDependencyIds.Add(jobId);
        }

        return this;
    }

    //public JobDescriptionBuilder RemoveJobDependencyId(string jobId)
    //{
    //    JobDescription.JobDependencyIds = JobDescription?.JobDependencyIds?.Where(id => id != jobId) ?? [];
    //    return this;
    //}

    public JobDescriptionBuilder AddJobGroupId(string jobGroupId)
    {
        _jobGroupIds.Add(jobGroupId);
        return this;
    }

    //public JobDescriptionBuilder RemoveJobGroupId(string jobGroupId)
    //{
    //    JobDescription.Groups = JobDescription?.Groups?.Where(id => id != jobGroupId) ?? [];
    //    return this;
    //}

    public JobDescriptionBuilder ApplyConfiguration(IConfiguration configuration)
    {
        JobDescriptionUtility.ApplyConfiguration(this, configuration);
        return this;
    }

    public JobDescriptionBuilder SetProperty(string key, object value)
    {
        _properties[key] = value;
        return this;
    }
}
