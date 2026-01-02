using System;

namespace ToolWheel;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public sealed class WaitForJobAttribute : Attribute
{
    public WaitForJobAttribute(string jobId)
    {
        JobId = jobId;
    }

    public string JobId { get; }
}
