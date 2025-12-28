using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace ToolWheel.Extensions.JobManager.Configuration;

public record JobDescription(MethodInfo Method) : IJobDescription
{
    public required string JobId { get; set; } = Guid.NewGuid().ToString();

    public IConfiguration? Configuration { get; set; } = null;

    public string JobName { get; set; } = Method.Name;

    public bool IsScoped { get; set; } = false;

    public int MaxExecutedJobs { get; set; } = 1;

    public bool Enabled { get; set; } = true;

    public IEnumerable<string>? JobDependencyIds { get; set; } = Array.Empty<string>();

    public IEnumerable<string> Groups { get; set; } = Array.Empty<string>();

    public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();
}
