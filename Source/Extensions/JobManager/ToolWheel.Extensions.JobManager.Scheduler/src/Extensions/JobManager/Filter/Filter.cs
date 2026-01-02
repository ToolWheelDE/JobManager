namespace ToolWheel.Extensions.JobManager.Filter;

public static class Filter
{
    public static IJobSchedulerFilter And(params IJobSchedulerFilter[] filters) => new AndFilter(filters);
    public static IJobSchedulerFilter Or(params IJobSchedulerFilter[] filters) => new OrFilter(filters);
    public static IJobSchedulerFilter Not(IJobSchedulerFilter filter) => new NotFilter(filter);
}
