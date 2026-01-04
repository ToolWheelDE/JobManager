using System.Linq;
using ToolWheel.Extensions.JobManager.Configuration;

namespace ToolWheel;

public static class JobDescriptorExtensions
{
    public static bool TryGetProperty<T>(this IJobDescription jobDescription, out T? value)
        where T : IFeature
    {
        value = GetProperty<T>(jobDescription);

        return value is not null;
    }

    public static T? GetProperty<T>(this IJobDescription jobDescription)
        where T : IFeature
    {
        var type = typeof(T);
        var result = jobDescription.Features.FirstOrDefault(f => f.GetType() == type);

        return result is null ? default : (T)result;
    }
}
