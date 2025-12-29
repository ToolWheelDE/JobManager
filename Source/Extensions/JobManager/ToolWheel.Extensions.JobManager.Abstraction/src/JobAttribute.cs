using System;

namespace ToolWheel;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class JobAttribute : Attribute
{
    public JobAttribute(string jobId)
    {
        JobId = jobId;
    }

    public string? Name { get; set; }

    public string? JobId { get; }

    public bool? IsScoped { get; set; }

    public int? MaxExecutedTasks { get; set; }
}
