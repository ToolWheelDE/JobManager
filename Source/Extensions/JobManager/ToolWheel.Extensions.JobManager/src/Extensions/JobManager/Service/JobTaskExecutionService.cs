using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    private readonly ILogger<JobTaskExecutionService>? _logger;
    private readonly IJobService _jobService;

    public JobTaskExecutionService(IServiceScopeFactory serviceScopeFactory, ILogger<JobTaskExecutionService>? logger, IJobService jobService)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _jobService = jobService;
    }

    public IJobTask? Execute(IJob job)
    {
        if (job is null)
        {
            throw new ArgumentNullException(nameof(job));
        }

        var jobTask = new JobTask(job, Guid.NewGuid().ToString());
        var cts = new CancellationTokenSource();

        jobTask.CancellationToken = cts;
        jobTask.ExecutionTask = Task.Run(() => UseProviderScopeAsync(jobTask, cts.Token), cts.Token);
        jobTask.ExecutionTask.ContinueWith(t =>
        {
            if (t.Exception != null)
            {
                _logger?.LogError(t.Exception, "Unhandled exception in job task {JobTaskId} for job {JobId}.", jobTask.Id, job.Id);
            }
        }, TaskContinuationOptions.OnlyOnFaulted);

        _logger?.LogInformation("Job task {JobTaskId} for job {JobId} started.", jobTask.Id, job.Id);

        return jobTask;
    }

    private async Task UseProviderScopeAsync(JobTask jobTask, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        using var loggerScope = _logger.BeginScope(new { jobTask.Id, jobTask.Job });
        using var jobLoggerScope = jobTask.Job.Logger?.BeginScope(new { jobTask.Id, jobTask.Job });

        var provider = scope.ServiceProvider;

        var asyncMiddlewares = provider.GetServices<IExecutionMiddlewareAsync>().ToArray();
        var jobTaskContextBuilder = new JobTaskContextBuilder(jobTask);

        Func<Task> pipeline = GenerateExecutionPipeline2(asyncMiddlewares, jobTaskContextBuilder, cancellationToken);

        try
        {
            await ExecuteMiddlewarePipelineAsync(provider, jobTaskContextBuilder, jobTask, pipeline, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            jobTask.CancellationToken?.Dispose();
        }
    }

    private Func<Task> GenerateExecutionPipeline2(IExecutionMiddlewareAsync[] asyncMiddlewares, JobTaskContextBuilder jobTaskContextBuilder, CancellationToken cancellationToken)
    {
        Func<Task> pipeline = () =>
        {
            _logger?.LogTrace("Job task pipeline was completed.");
            return Task.CompletedTask;
        };

        // Asynchrone Middlewares in umgekehrter Reihenfolge einfügen.
        for (int i = asyncMiddlewares.Length - 1; i >= 0; i--)
        {
            var middleware = asyncMiddlewares[i];
            var next = pipeline;
            pipeline = () =>
            {
                _logger?.LogTrace("Executing async middleware {Middleware}.", middleware.GetType().Name);
                return middleware.InvokeAsync(jobTaskContextBuilder, next, cancellationToken);
            };
        }

        return pipeline;
    }

    private async Task ExecuteMiddlewarePipelineAsync(IServiceProvider provider, JobTaskContextBuilder jobTaskContextBuilder, JobTask jobTask, Func<Task> pipeline, CancellationToken cancellationToken)
    {
        _logger?.LogDebug("Executing stage pipeline.");

        jobTask.Status = JobTaskStatus.Running;
        jobTask.StartTimesstamp = DateTime.UtcNow;

        var jobTaskContextAccessor = provider.GetRequiredService<IJobTaskContextAccessor>() as JobTaskContextAccessor ?? throw new InvalidOperationException("JobTaskContextAccessor is not writable.");
        jobTaskContextAccessor.JobTaskContext = jobTaskContextBuilder.Build();

        try
        {
            await pipeline().ConfigureAwait(false);

            jobTask.Status = JobTaskStatus.Success;
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            _logger?.LogWarning(ex.InnerException, "Invocation exception during job task pipeline execution for {JobTaskId}.", jobTask.Id);
            if (jobTask.Status == JobTaskStatus.Running)
            {
                jobTask.Status = JobTaskStatus.Failed;
            }
        }
        catch (JobTaskExecutionException ex)
        {
            _logger?.LogWarning(ex, "Job task execution exception for {JobTaskId}.", jobTask.Id);
            if (jobTask.Status == JobTaskStatus.Running)
            {
                jobTask.Status = JobTaskStatus.Failed;
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger?.LogInformation("Job task {JobTaskId} was cancelled.", jobTask.Id);
            jobTask.Status = JobTaskStatus.Failed;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "An error occurred during job task pipeline execution.");
            jobTask.Status = JobTaskStatus.Broken;
        }
        finally
        {
            jobTask.CompletTimesstamp = DateTime.UtcNow;
            _jobService.SetLastExecutionTimestamp(jobTask.Job, jobTask.SignalTimesstamp);
        }
    }
}