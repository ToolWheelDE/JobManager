using System;
using ToolWheel.Extensions.JobManager.Configuration;

namespace ToolWheel;

public static class JobDescriptorExtensions
{
    public static void SetProperty(this IJobDescription jobDescription, string key, object value)
    {
        jobDescription.Properties[key] = value;
    }

    public static bool TryGetProperty(this IJobDescription jobDescription, string key, out object? value)
    {
        return jobDescription.Properties.TryGetValue(key, out value);
    }

    public static object GetProperty(this IJobDescription jobDescription, string key)
    {
        return jobDescription.Properties[key];
    }

    public static bool TryGetProperty<T>(this IJobDescription jobDescription, string key, out T? value)
    {
        var result = jobDescription.Properties.TryGetValue(key, out var objValue);

        value = result ? (T?)Convert.ChangeType(objValue, typeof(T)) : default;

        return result;
    }

    public static T GetProperty<T>(this IJobDescription jobDescription, string key, T? defaultValue = default)
    {
        if (!jobDescription.TryGetProperty(key, out var value))
        {
            return defaultValue!;
        }

        return (T)Convert.ChangeType(value, typeof(T));
    }
}
