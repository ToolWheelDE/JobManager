using System;
using System.Collections.Generic;
using ToolWheel.Extensions.JobManager.Configuration;

namespace ToolWheel.Extensions.JobManager.Service;

public interface IJobService
{
    IJob? Add(IJobDescription job);

    IJob? Find(string jobId);

    IEnumerable<IJob> Read();

    JobStatus UpdateJobStatus(IJob job);

    void SetLastExecutionTimestamp(IJob job, DateTime timestamp);

    void AddJobDependency(IJob job, IJob waitFor);

    void RemoveJobDependency(IJob job, IJob waitFor);
}
