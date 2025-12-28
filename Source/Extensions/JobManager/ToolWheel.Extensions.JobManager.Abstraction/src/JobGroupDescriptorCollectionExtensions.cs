using ToolWheel.Extensions.JobManager.Configuration;

namespace ToolWheel;

public static class JobGroupDescriptorCollectionExtensions
{
    public static JobGroupDescriptionCollection Add(this JobGroupDescriptionCollection collection, string groupId, string groupName, int maxExecutedJobs)
    {
        var descriptor = new JobGroupDescription(groupId)
        {
            GroupName = groupName,
            MaxExecutedJobs = maxExecutedJobs
        };

        collection.Add(descriptor);

        return collection;
    }
}
