using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using ToolWheel;

namespace ToolWheel.Extensions.JobManager.Configuration;
public class JobDescriptionUtility
{
    public static JobDescription CreateJobDescription(MethodInfo method, Func<MethodInfo, string> jobIdResolver, Action<JobDescriptionBuilder>? configure = null)
    {
        var jobDescription = new JobDescription(method)
        {
            JobId = jobIdResolver(method) 
        };

        ApplyJobDependencyAttributes(jobDescription);
        configure?.Invoke(new JobDescriptionBuilder(jobDescription));

        return jobDescription;
    }

    public static IEnumerable<JobDescription> CreateJobDescriptions(MethodInfo method, Func<JobDescriptionIdInfo, string> jobIdResolver)
    {
        var jobAttributes = method.GetCustomAttributes<JobAttribute>();

        if (jobAttributes.Any())
        {
            foreach (var jobAttribute in jobAttributes)
            {
                var info = new JobDescriptionIdInfo(method, jobAttribute);
                var jobDescription = new JobDescription(method)
                {
                    JobId = jobIdResolver(info)
                };

                ApplyJobAttribute(jobDescription, jobAttribute);
                ApplyJobDependencyAttributes(jobDescription);

                yield return jobDescription;
            }
        }
        else
        {
            var info = new JobDescriptionIdInfo(method, null);
            var jobDescription = new JobDescription(method)
            {
                JobId = jobIdResolver(info)
            };

            ApplyJobDependencyAttributes(jobDescription);

            yield return jobDescription;
        }
    }

    private static void ApplyJobAttribute(JobDescription jobDescription, JobAttribute jobAttribute)
    {
        jobDescription.JobId = jobAttribute.JobId ?? jobDescription.JobId;
        jobDescription.JobName = jobAttribute.Name ?? jobDescription.JobName;
        jobDescription.IsScoped = jobAttribute.IsScoped;
        jobDescription.MaxExecutedJobs = jobAttribute.MaxExecutedTasks;
    }

    private static void ApplyJobDependencyAttributes(JobDescription jobDescription)
    {
        jobDescription.JobDependencyIds = jobDescription.Method.GetCustomAttributes<WaitForJobAttribute>().Select(attr => attr.JobId);
    }

    public static void ApplyConfiguration(IJobDescription jobDescription, IConfiguration? configuration)
    {
        jobDescription.Configuration = configuration;

        if (configuration is not null)
        {
            jobDescription.JobName = configuration.GetValue<string?>(nameof(JobDescription.JobName)) ?? jobDescription.JobName;
            jobDescription.Enabled = configuration.GetValue<bool?>(nameof(JobDescription.Enabled)) ?? jobDescription.Enabled;
            jobDescription.MaxExecutedJobs = configuration.GetValue<int?>(nameof(JobDescription.MaxExecutedJobs)) ?? jobDescription.MaxExecutedJobs;

            var section = configuration.GetSection("WaitForJobs");
            jobDescription.JobDependencyIds = section.Exists() ? section.Get<IEnumerable<string>>() : [];
        }
    }
}

public record struct JobDescriptionIdInfo(MethodInfo Method, JobAttribute? Attribute)
{ }
