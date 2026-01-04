using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace ToolWheel.Extensions.JobManager.Configuration;

public class JobDescriptionBuilder
{
    private readonly MethodInfo _methodInfo;
    private object? _target;
    private string _id;
    private string? _name;
    private int? _maxExecutedTasks;
    private bool? _isScoped;
    private bool? _enabled;
    private readonly HashSet<string> _jobDependencyIds = new HashSet<string>();
    private readonly HashSet<string> _jobGroupIds = new HashSet<string>();
    private readonly HashSet<IFeature> _features = new HashSet<IFeature>();

    public JobDescriptionBuilder(MethodInfo methodInfo, object? target, string? id = null)
    {
        _methodInfo = methodInfo;
        _target=target;
        _id = id ?? Guid.NewGuid().ToString ();
    }

    public JobDescription Build()
    {
        if (string.IsNullOrWhiteSpace(_id))
        {
            throw new InvalidOperationException("Job ID must be specified.");
        }

        return new JobDescription(_id)
        {
            Method = _methodInfo,
            Target = _target,
            JobName = _name ?? _methodInfo.Name,
            MaxExecutedJobs = _maxExecutedTasks ?? 0,
            IsScoped = _isScoped ?? true,
            Enabled = _enabled ?? true,
            JobDependencyIds = _jobDependencyIds.ToArray(),
            Groups = _jobGroupIds.ToArray(),
            Features = _features.ToArray()
        };
    }

    public JobDescriptionIdInfo BuildInfo()
    {
        return new JobDescriptionIdInfo
        {
            Target = _methodInfo,
            Id = _id!
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

    public JobDescriptionBuilder AddJobGroupId(string jobGroupId)
    {
        _jobGroupIds.Add(jobGroupId);
        return this;
    }

    public JobDescriptionBuilder ApplyConfiguration(IConfiguration configuration)
    {
        JobDescriptionConfigurationUtility.ApplyConfiguration(this, configuration);
        return this;
    }

    public JobDescriptionBuilder AddFeature(IFeature feature)
    {
        _features.Add(feature);

        return this;
    }
}
