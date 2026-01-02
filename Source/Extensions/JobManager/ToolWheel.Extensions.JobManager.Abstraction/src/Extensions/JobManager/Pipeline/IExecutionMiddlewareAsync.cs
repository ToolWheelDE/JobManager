using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ToolWheel.Extensions.JobManager;

namespace ToolWheel.Extensions.JobManager.Pipeline;

public interface IExecutionMiddlewareAsync
{
    /// <summary>
    /// Führt die Middleware asynchron aus.
    /// </summary>
    /// <param name="context">Der Pipeline-Kontext/Builder.</param>
    /// <param name="next">Die Funktion, die die nächste Stufe ausführt.</param>
    /// <param name="cancellationToken">CancellationToken zur Abbruchunterstützung.</param>
    Task InvokeAsync(IJobTaskContextBuilder context, Func<Task> next, CancellationToken cancellationToken);
}
