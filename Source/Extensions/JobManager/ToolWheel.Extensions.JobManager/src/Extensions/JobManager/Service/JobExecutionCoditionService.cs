using System.Collections.Generic;
using ToolWheel.Extensions.JobManager.Pipeline;

namespace ToolWheel.Extensions.JobManager.Service;
public class JobExecutionCoditionService
{
    private readonly IEnumerable<IExecutionCondition> _executionConditions;

    public JobExecutionCoditionService(IEnumerable<IExecutionCondition> executionConditions)
    {
        _executionConditions = executionConditions;
    }
}
