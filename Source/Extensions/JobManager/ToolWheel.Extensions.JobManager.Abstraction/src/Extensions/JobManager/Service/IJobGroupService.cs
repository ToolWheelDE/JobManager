using System.Collections.Generic;
using ToolWheel.Extensions.JobManager.Configuration;

namespace ToolWheel.Extensions.JobManager.Service;

public interface IJobGroupService
{
    IJobGroup? Add(IJobGroupDescription jobGroupDescription);

    void AddMember(IJobGroup jobGroup, IJob jobDescription);

    void RemoveMember(IJobGroup jobGroup, IJob job);

    IJobGroup GetJobGroup(string jobGroupId);

    IEnumerable<IJob> GetJobGroupMembers(IJobGroup jobGroup);

    IEnumerable<IJobGroup> GetJobGroupMembers(IJob job);
}
