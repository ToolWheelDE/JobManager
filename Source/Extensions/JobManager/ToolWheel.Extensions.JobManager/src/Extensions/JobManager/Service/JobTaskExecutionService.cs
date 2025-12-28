using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ToolWheel.Extensions.JobManager;
using ToolWheel.Extensions.JobManager.Pipeline;

namespace ToolWheel.Extensions.JobManager.Service;

public class JobTaskExecutionService : IJobTaskExecutionService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<JobTaskExecutionService> _logger;
    private readonly IJobService _jobService;

    public JobTaskExecutionService(IServiceScopeFactory serviceScopeFactory, ILogger<JobTaskExecutionService> logger, IJobService jobService)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _jobService = jobService;
    }

    public IJobTask? Execute(IJob job)
    {
        if (job is null) throw new ArgumentNullException(nameof(job));

        var jobTask = new JobTask(job, Guid.NewGuid().ToString());

        var cts = new CancellationTokenSource();
        jobTask.CancellationToken = cts;

        jobTask.ExecutionTask = Task.Run(() => GenerateExecutionPipelineAsync(jobTask, cts.Token), cts.Token);

        jobTask.ExecutionTask.ContinueWith(t =>
        {
            if (t.Exception != null)
            {
                _logger.LogError(t.Exception, "Unhandled exception in job task {JobTaskId} for job {JobId}.", jobTask.Id, job.Id);
            }
        }, TaskContinuationOptions.OnlyOnFaulted);

        _logger.LogInformation("Job task {JobTaskId} for job {JobId} started.", jobTask.Id, job.Id);

        return jobTask;
    }

    private async Task GenerateExecutionPipelineAsync(JobTask jobTask, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        using var loggerScope = _logger.BeginScope(new { jobTask.Id, jobTask.Job });
        using var jobLoggerScope = jobTask.Job.Logger?.BeginScope(new { jobTask.Id, jobTask.Job });

        var provider = scope.ServiceProvider;

        var asyncMiddlewares = provider.GetServices<IExecutionMiddlewareAsync>().ToArray();
        var jobTaskJournalService = provider.GetRequiredService<IJobTaskJournalService>();

        var jobTaskContextAccessor = provider.GetRequiredService<IJobTaskContextAccessor>() as JobTaskContextAccessor
            ?? throw new InvalidOperationException("JobTaskContextAccessor is not writable.");
        var jobTaskContext = new JobTaskContext(jobTask);
        var jobTaskContextBuilder = new JobTaskContextBuilder(jobTask, jobTaskContext);
        var journalList = new List<IJournalEntry>();

        jobTaskJournalService.AddTask(jobTask, journalList);
        jobTaskContextAccessor.JobTaskContext = jobTaskContextBuilder;
        jobTaskContext.Journal = new JournalLogger(jobTask.Job.Logger, journalList);

        Func<Task> pipeline = () =>
        {
            _logger.LogTrace("Job task pipeline was completed.");
            return Task.CompletedTask;
        };

        // Asynchrone Middlewares in umgekehrter Reihenfolge einfügen.
        for (int i = asyncMiddlewares.Length - 1; i >= 0; i--)
        {
            var middleware = asyncMiddlewares[i];
            var next = pipeline;
            pipeline = () =>
            {
                _logger.LogTrace("Executing async middleware {Middleware}.", middleware.GetType().Name);
                return middleware.InvokeAsync(jobTaskContextBuilder, next, cancellationToken);
            };
        }

        try
        {
            await ExecuteMiddlewarePipelineAsync(jobTask, pipeline, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            jobTask.CancellationToken?.Dispose();
        }
    }

    private async Task ExecuteMiddlewarePipelineAsync(JobTask jobTask, Func<Task> pipeline, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Executing stage pipeline.");

            jobTask.Status = JobTaskStatus.Running;
            jobTask.StartTimesstamp = DateTime.UtcNow;

            await pipeline().ConfigureAwait(false);

            if (jobTask.Status == JobTaskStatus.Running)
            {
                jobTask.Status = JobTaskStatus.Success;
            }
        }
        catch (JobTaskExecutionException ex)
        {
            _logger.LogWarning(ex, "Job task execution exception for {JobTaskId}.", jobTask.Id);
            if (jobTask.Status == JobTaskStatus.Running)
            {
                jobTask.Status = JobTaskStatus.Failed;
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Job task {JobTaskId} was cancelled.", jobTask.Id);
            jobTask.Status = JobTaskStatus.Failed;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during job task pipeline execution.");
            jobTask.Status = JobTaskStatus.Broken;
        }
        finally
        {
            jobTask.CompletTimesstamp = DateTime.UtcNow;
            _jobService.SetLastExecutionTimestamp(jobTask.Job, jobTask.SignalTimesstamp);
        }
    }
}
