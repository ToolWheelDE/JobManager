using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace ToolWheel.Extensions.JobManager.Configuration;
public interface IJobDescription
{
    MethodInfo Method { get; init; }

    string JobId { get; set; }

    bool IsScoped { get; set; }

    IConfiguration? Configuration { get; set; }

    string JobName { get; set; }

    int MaxExecutedJobs { get; set; }

    bool Enabled { get; set; }

    IEnumerable<string>? JobDependencyIds { get; set; }

    IEnumerable<string> Groups { get; set; }

    IDictionary<string, object> Properties { get; }
}
