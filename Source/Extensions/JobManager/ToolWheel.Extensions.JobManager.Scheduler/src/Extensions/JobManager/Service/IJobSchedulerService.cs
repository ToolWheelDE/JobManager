using System;
using System.Collections.Generic;
using ToolWheel.Extensions.JobManager.Configuration;

namespace ToolWheel.Extensions.JobManager.Service;

public interface IJobSchedulerService
{
    IJobScheduler Add(IJob job, Action<JobScheduleBuilder> configure);

    IJobScheduler Add(IJob job, IJobSchedulerDescription jobSchedulerDescription);

    IJobScheduler? GetJobSchedulers(IJob job);

    IEnumerable<IJobScheduler> GetJobSchedulers();
}
