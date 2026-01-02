using System.Reflection;

namespace ToolWheel.Extensions.JobManager.Configuration;

public record struct JobDescriptionIdInfo(MethodInfo Target, string? Id)
{ }
