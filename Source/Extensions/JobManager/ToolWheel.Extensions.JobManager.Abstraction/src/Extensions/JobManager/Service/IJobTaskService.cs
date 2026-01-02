using System.Collections.Generic;
using ToolWheel.Extensions.JobManager;

namespace ToolWheel.Extensions.JobManager.Service;

public interface IJobTaskService
{
    IJobTask? Execute(IJob job);

    IJobTask? Find(string jobTaskId);

    IEnumerable<IJobTask> Read(IJob job);

    IEnumerable<IJobTask> ReadByStatus(IJob job, JobTaskStatus status);

    void WaitAllTasks();
}
