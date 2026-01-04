using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToolWheel.Extensions.JobManager.Service;

public class JobTaskService : IJobTaskService
{
    private readonly Dictionary<IJob, List<IJobTask>> _tasks = new Dictionary<IJob, List<IJobTask>>();
    private readonly IJobService _jobService;
    private readonly IJobTaskExecutionService _jobTaskExecutionService;

    public JobTaskService(IJobService jobService, IJobTaskExecutionService jobTaskExecutionService)
    {
        _jobService = jobService;
        _jobTaskExecutionService = jobTaskExecutionService;
    }

    public IJobTask? Execute(IJob job)
    {
        if (job.Status != JobStatus.Ready)
        {
            return null;
        }

        var task = _jobTaskExecutionService.Execute(job);

        if (task is not null)
        {
            GetListFromDictionary(job).Add(task);
        }

        return task;
    }

    public IJobTask? Find(string jobTaskId)
    {
        return _tasks.Values.SelectMany(m => m)
            .FirstOrDefault(m => m.Id == jobTaskId);
    }

    public IEnumerable<IJobTask> Read(IJob job)
    {
        return GetListFromDictionary(job).ToArray();
    }

    public IEnumerable<IJobTask> ReadByStatus(IJob job, JobTaskStatus status)
    {
        return GetListFromDictionary(job).Where(m => m.Status == status).ToArray();
    }

    public void WaitAllTasks()
    {
        var taskArray = _tasks
            .SelectMany(m => m.Value)
            .Where(m => m is not null && (m.Status == JobTaskStatus.Running || m.Status == JobTaskStatus.Pending))
            .Select(m => m.ExecutionTask)
            .ToArray();

        Task.WaitAll(taskArray!);
    }

    private List<IJobTask> GetListFromDictionary(IJob job)
    {
        if (!_tasks.TryGetValue(job, out var list))
        {
            list = new List<IJobTask>();
            _tasks.Add(job, list);
        }

        return list;
    }
}
