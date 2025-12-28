using System.Linq;
using Microsoft.Extensions.Configuration;

namespace ToolWheel.Extensions.JobManager.Configuration;
public class JobDescriptionBuilder
{
    public JobDescriptionBuilder(JobDescription jobDescription)
    {
        JobDescription = jobDescription;
    }

    public JobDescription JobDescription { get; }

    public JobDescriptionBuilder Id(string jobId)
    {
        JobDescription.JobId = jobId;
        return this;
    }

    public JobDescriptionBuilder Name(string jobName)
    {
        JobDescription.JobName = jobName;
        return this;
    }

    public JobDescriptionBuilder MaxExecutedTasks(int count)
    {
        JobDescription.MaxExecutedJobs = count;
        return this;
    }

    public JobDescriptionBuilder IsScoped()
    {
        JobDescription.IsScoped = true;
        return this;
    }

    public JobDescriptionBuilder IsSingleton()
    {
        JobDescription.IsScoped = false;
        return this;
    }

    public JobDescriptionBuilder Enabled()
    {
        JobDescription.Enabled = true;
        return this;
    }

    public JobDescriptionBuilder Disabled()
    {
        JobDescription.Enabled = false;
        return this;
    }

    public JobDescriptionBuilder AddJobDependencyId(string jobId)
    {
        JobDescription.JobDependencyIds = JobDescription?.JobDependencyIds?.Append(jobId) ?? [jobId];
        return this;
    }

    public JobDescriptionBuilder RemoveJobDependencyId(string jobId)
    {
        JobDescription.JobDependencyIds = JobDescription?.JobDependencyIds?.Where(id => id != jobId) ?? [];
        return this;
    }

    public JobDescriptionBuilder AddJobGroupId(string jobGroupId)
    {
        JobDescription.Groups = JobDescription?.Groups?.Append(jobGroupId) ?? [jobGroupId];
        return this;
    }

    public JobDescriptionBuilder RemoveJobGroupId(string jobGroupId)
    {
        JobDescription.Groups = JobDescription?.Groups?.Where(id => id != jobGroupId) ?? [];
        return this;
    }

    public JobDescriptionBuilder ApplyConfiguration(IConfiguration configuration)
    {
        JobDescriptionUtility.ApplyConfiguration(JobDescription, configuration);
        return this;
    }

    public JobDescriptionBuilder SetProperty(string key, object value)
    {
        JobDescription.Properties[key] = value;
        return this;
    }
}
