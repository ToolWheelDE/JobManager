using System;
using System.Collections.Generic;
using ToolWheel.Extensions.JobManager.Filter;

namespace ToolWheel.Extensions.JobManager.Configuration;

public sealed class FilterBuilder : IFilterBuilder
{
    private readonly List<IJobSchedulerFilter> _filters = new();
    private readonly GroupMode _mode;

    public FilterBuilder(GroupMode mode = GroupMode.And)
    {
        _mode = mode;
    }

    public IFilterBuilder Add(IJobSchedulerFilter filter)
    {
        if (filter is null)
        {
            throw new ArgumentNullException(nameof(filter));
        }

        _filters.Add(filter);

        return this;
    }

    public IFilterBuilder And(Action<IFilterBuilder> group)
    {
        if (group is null)
        {
            throw new ArgumentNullException(nameof(group));
        }

        var child = new FilterBuilder(GroupMode.And);
        group(child);

        _filters.Add(child.BuildInternal());

        return this;
    }

    public IFilterBuilder Or(Action<IFilterBuilder> group)
    {
        if (group is null)
        {
            throw new ArgumentNullException(nameof(group));
        }

        var child = new FilterBuilder(GroupMode.Or);
        group(child);

        _filters.Add(child.BuildInternal());

        return this;
    }

    public IFilterBuilder Not(Action<IFilterBuilder> group)
    {
        if (group is null)
        {
            throw new ArgumentNullException(nameof(group));
        }

        var child = new FilterBuilder(GroupMode.And);
        group(child);

        var inner = child.BuildInternal();
        _filters.Add(new NotFilter(inner));

        return this;
    }

    public IFilterBuilder And(IJobSchedulerFilter filter)
    {
        if (filter is null)
        {
            throw new ArgumentNullException(nameof(filter));
        }

        if (_mode == GroupMode.And)
        {
            return Add(filter);
        }

        var group = new FilterBuilder(GroupMode.And)
            .Add(filter)
            .Build();

        _filters.Add(group);
        return this;
    }

    public IFilterBuilder Or(IJobSchedulerFilter filter)
    {
        if (filter is null)
        {
            throw new ArgumentNullException(nameof(filter));
        }

        if (_mode == GroupMode.Or)
        {
            return Add(filter);
        }

        var group = new FilterBuilder(GroupMode.Or)
            .Add(filter)
            .Build();

        _filters.Add(group);

        return this;
    }

    public IJobSchedulerFilter Build()
    {
        return BuildInternal();
    }

    private IJobSchedulerFilter BuildInternal()
    {
        if (_filters.Count == 0)
        {
            return new AllowAllFilter();
        }

        if (_filters.Count == 1)
        {
            return _filters[0];
        }

        return _mode switch
        {
            GroupMode.And => new AndFilter(_filters),
            GroupMode.Or => new OrFilter(_filters),
            _ => throw new InvalidOperationException("Unsupported mode")
        };
    }
}
