using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ToolWheel.Extensions.JobManager;
using ToolWheel.Extensions.JobManager.Service;

namespace ToolWheel.Extensions.JobManager.Pipeline;

public class JobExecutionMiddleware : IExecutionMiddlewareAsync
{
    private readonly IJobInstanceService _jobInstanceService;
    private readonly IServiceProvider _serviceProvider;

    public JobExecutionMiddleware(IJobInstanceService jobInstanceService, IServiceProvider serviceProvider)
    {
        _jobInstanceService = jobInstanceService;
        _serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(IJobTaskContextBuilder contextBuilder, Func<Task> next, CancellationToken cancellationToken)
    {
        var instance = CreateInstance(contextBuilder, cancellationToken);
        var methodParameters = ResolveMethodParameters(contextBuilder.JobTask.Job.Method, contextBuilder, cancellationToken).ToArray();

        try
        {
            contextBuilder.Journal?.LogInformation("Job Task started.");

            cancellationToken.ThrowIfCancellationRequested();

            var result = contextBuilder.JobTask.Job.Method.Invoke(instance, methodParameters);

            if (result is Task taskResult)
            {
                await taskResult.ConfigureAwait(false);
            }
            else if (result != null)
            {
                var rt = result.GetType();
                var isValueTask = rt == typeof(ValueTask) || rt.IsGenericType && rt.GetGenericTypeDefinition() == typeof(ValueTask<>);

                if (isValueTask)
                {
                    var asTask = rt.GetMethod("AsTask", BindingFlags.Public | BindingFlags.Instance);
                    if (asTask != null)
                    {
                        var asTaskObj = (Task)asTask.Invoke(result, null)!;
                        await asTaskObj.ConfigureAwait(false);
                    }
                    else
                    {
                        // Fallback: dynamisches Await (praktisch und kurz)
                        await (dynamic)result;
                    }
                }
            }

            contextBuilder.Journal?.LogInformation("Job Task completed in {runtime}", contextBuilder.JobTask.Runtime);
            contextBuilder.Status = JobTaskStatus.Success;
        }
        catch (TargetInvocationException ex)
        {
            contextBuilder.Status = JobTaskStatus.Failed;

            // Wenn die aufgerufene Methode abgebrochen wurde, Propagiere die OCE direkt, damit Cancellation erkannt wird.
            if (ex.InnerException is OperationCanceledException oce && cancellationToken.IsCancellationRequested)
            {
                throw oce;
            }

            throw new JobTaskExecutionException(contextBuilder.JobTask, "An error occurred while executing the job", ex.InnerException ?? ex);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            contextBuilder.Status = JobTaskStatus.Failed;
            throw;
        }
        catch (Exception ex)
        {
            contextBuilder.Status = JobTaskStatus.Failed;
            throw new JobTaskExecutionException(contextBuilder.JobTask, "An error occurred while executing the job", ex);
        }

        // Weiter zur nächsten Middleware
        if (next != null)
        {
            await next().ConfigureAwait(false);
        }
    }

    private object CreateInstance(IJobTaskContext jobTaskContext, CancellationToken cancellationToken)
    {
        if (jobTaskContext.JobTask.Job.IsScopedInstance)
        {
            var constructors = jobTaskContext.JobTask.Job.Method.DeclaringType!.GetConstructors();
            if (constructors.Length > 1)
            {
                throw new InvalidOperationException($"Job class '{jobTaskContext.JobTask.Job.Method.DeclaringType!.FullName}' has more than one constructor. Only one constructor is allowed.");
            }

            var parametersArray = ResolveMethodParameters(constructors[0], jobTaskContext, cancellationToken).ToArray();

            return Activator.CreateInstance(jobTaskContext.JobTask.Job.Method.DeclaringType!, parametersArray)!;
        }

        return _jobInstanceService.GetJobInstance(jobTaskContext.JobTask.Job)!;
    }

    private IEnumerable<object?> ResolveMethodParameters(MethodBase method, IJobTaskContext context, CancellationToken cancellationToken)
    {
        foreach (var parameter in method.GetParameters())
        {
            var type = parameter.ParameterType;

            if (typeof(IJob).IsAssignableFrom(type)) yield return context.JobTask.Job;
            else if (typeof(IJobTask).IsAssignableFrom(type)) yield return context.JobTask;
            else if (typeof(IJobTaskContext).IsAssignableFrom(type)) yield return context;
            else if (typeof(IJobLogger).IsAssignableFrom(type)) yield return context.Journal;
            else if (type == typeof(CancellationToken)) yield return cancellationToken;
            else yield return _serviceProvider.GetService(type);
        }
    }
}
