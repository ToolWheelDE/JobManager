using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace ToolWheel.Extensions.JobManager.Configuration;

public class JobDescriptionUtility
{
    public static JobDescription CreateJobDescription(MethodInfo method, Func<MethodInfo, string> jobIdResolver, Action<JobDescriptionBuilder>? configure = null)
    {
        var builder = new JobDescriptionBuilder(method, jobIdResolver(method));

        ApplyJobDependencyAttributes(builder, method);

        configure?.Invoke(builder);

        return builder.Build();
    }

    public static IEnumerable<JobDescription> CreateJobDescriptions(MethodInfo target, Func<JobDescriptionIdInfo, string> jobIdResolver)
    {
        var jobAttributes = target.GetCustomAttributes<JobAttribute>();

        if (jobAttributes.Any())
        {
            foreach (var jobAttribute in jobAttributes)
            {
                var info = new JobDescriptionIdInfo(target, jobAttribute);
                var builder = new JobDescriptionBuilder(target, jobIdResolver(info)); ;

                ApplyJobAttribute(builder, jobAttribute);
                ApplyJobDependencyAttributes(builder, target);

                yield return builder.Build();
            }
        }
        else
        {
            var info = new JobDescriptionIdInfo(target, null);
            var builder = new JobDescriptionBuilder(target, jobIdResolver(info));

            ApplyJobDependencyAttributes(builder, target);

            yield return builder.Build();
        }
    }

    private static void ApplyJobAttribute(JobDescriptionBuilder builder, JobAttribute jobAttribute)
    {
        if (jobAttribute is null)
        {
            throw new ArgumentNullException(nameof(jobAttribute));
        }

        if (jobAttribute.JobId is not null)
        {
            builder.Id(jobAttribute.JobId);
        }

        if (jobAttribute.Name is not null)
        {
            builder.Name(jobAttribute.Name);
        }

        if (jobAttribute.IsScoped is not null && jobAttribute.IsScoped == true)
        {
            builder.IsScoped();
        }

        if (jobAttribute.MaxExecutedTasks is not null)
        {
            builder.MaxExecutedTasks(jobAttribute.MaxExecutedTasks.Value);
        }
    }

    private static void ApplyJobDependencyAttributes(JobDescriptionBuilder jobDescription, MethodInfo target)
    {
        jobDescription.AddJobDependencyId(target.GetCustomAttributes<WaitForJobAttribute>().Select(attr => attr.JobId));
    }

    public static void ApplyConfiguration(JobDescriptionBuilder builder, IConfiguration? configuration)
    {
        if (configuration is not null)
        {
            var name = configuration.GetValue<string?>(nameof(JobDescription.JobName));
            var enabled = configuration.GetValue<bool?>(nameof(JobDescription.Enabled));
            var maxExecutedJobs = configuration.GetValue<int?>(nameof(JobDescription.MaxExecutedJobs));

            if (name is not null)
            {
                builder.Name(name);
            }

            if (enabled is not null)
            {
                builder.Enabled();
            }

            if (maxExecutedJobs is not null)
            {
                builder.MaxExecutedTasks(maxExecutedJobs.Value);
            }
        }
    }
}

public record struct JobDescriptionIdInfo(MethodInfo Method, JobAttribute? Attribute)
{ }
