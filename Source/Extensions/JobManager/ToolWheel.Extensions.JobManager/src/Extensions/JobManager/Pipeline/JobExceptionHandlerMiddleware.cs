using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ToolWheel.Extensions.JobManager;
using ToolWheel.Extensions.JobManager.Service;

namespace ToolWheel.Extensions.JobManager.Pipeline;

public class JobExceptionHandlerMiddleware : IExecutionMiddlewareAsync
{
    private readonly ILogger<JobExceptionHandlerMiddleware> _logger;

    public JobExceptionHandlerMiddleware(ILogger<JobExceptionHandlerMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(IJobTaskContextBuilder contextBuilder, Func<Task> next, CancellationToken cancellationToken)
    {
        try
        {
            await next().ConfigureAwait(false);
        }
        catch (JobTaskExecutionException ex)
        {
            var exception = ex?.InnerException ?? ex;

            _logger.LogError(exception, "Job Task Execution Exception.");
            contextBuilder.Journal?.LogError(exception, "Job Task Execution Exception.");

            contextBuilder.Status = JobTaskStatus.Failed;
        }
        catch (Exception)
        {
            contextBuilder.Status = JobTaskStatus.Failed;

            throw;
        }
    }
}
