using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ToolWheel.Extensions.JobManager.Service;
using Microsoft.Extensions.Logging;

namespace ToolWheel.Extensions.JobManager.Pipeline;
public class JobTaskRetryMiddleware : IExecutionMiddlewareAsync
{
    private readonly ILogger<JobTaskRetryMiddleware> _logger;
    private readonly IJobTaskRetryService _jobTaskRetryService;

    public JobTaskRetryMiddleware(ILogger<JobTaskRetryMiddleware> logger, IJobTaskRetryService jobTaskRetryService)
    {
        _logger = logger;
        _jobTaskRetryService = jobTaskRetryService;
    }

    public async Task InvokeAsync(IJobTaskContextBuilder contextBuilder, Func<Task> next, CancellationToken cancellationToken)
    {
        var strategy = _jobTaskRetryService.Get(contextBuilder.JobTask.Job);
        var counter = 0;

        if (strategy is not null)
        {
            while (true)
            {
                counter++;

                await next().ConfigureAwait(false);

                if (contextBuilder.JobTask.Status == JobTaskStatus.Success)
                {
                    return;
                }

                if (counter >= strategy.RetryCount)
                {
                    _logger.LogWarning("Execution has failed {count} times, task execution is aborted.", counter);
                    break;
                }

                if (strategy.RetryDelay is not null)
                {
                    _logger.LogWarning("Execution {count} has failed and is repeated in {timespan}.", counter, strategy.RetryDelay);

                    try
                    {
                        await Task.Delay(strategy.RetryDelay.Value, cancellationToken).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogInformation("Retry delay cancelled by token.");
                        throw;
                    }
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Invocation cancelled by token.");
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }
        else
        {
            await next().ConfigureAwait(false);
        }
    }
}
