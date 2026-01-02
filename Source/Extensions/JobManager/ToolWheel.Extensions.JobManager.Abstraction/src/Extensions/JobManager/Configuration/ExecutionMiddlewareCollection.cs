using System;
using System.Collections;
using System.Collections.Generic;
using ToolWheel.Extensions.JobManager.Pipeline;

namespace ToolWheel.Extensions.JobManager.Configuration;
public class ExecutionMiddlewareCollection : IEnumerable<Type>
{
    private readonly LinkedList<Type> _middlewares = new LinkedList<Type>();

    public void AddFirst<T>()
        where T : IExecutionMiddlewareAsync
    {
        AddFirst(typeof(T));
    }

    public void AddFirst(Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        if (!typeof(IExecutionMiddlewareAsync).IsAssignableFrom(type))
        {
            throw new ArgumentException($"The type {type.Name} does not implement {nameof(IExecutionMiddlewareAsync)}");
        }

        _middlewares.AddFirst(type);
    }

    public void AddLast<T>()
        where T : IExecutionMiddlewareAsync
    {
        AddLast(typeof(T));
    }

    public void AddLast(Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        if (!typeof(IExecutionMiddlewareAsync).IsAssignableFrom(type))
        {
            throw new ArgumentException($"The type {type.Name} does not implement {nameof(IExecutionMiddlewareAsync)}");
        }

        _middlewares.AddLast(type);
    }

    public void AddBefore<TBefore, TAfter>()
        where TBefore : IExecutionMiddlewareAsync
        where TAfter : IExecutionMiddlewareAsync
    {
        AddBefore(typeof(TBefore), typeof(TAfter));
    }

    public void AddBefore(Type before, Type after)
    {
        if (before == null)
        {
            throw new ArgumentNullException(nameof(before));
        }

        if (after == null)
        {
            throw new ArgumentNullException(nameof(after));
        }

        if (!typeof(IExecutionMiddlewareAsync).IsAssignableFrom(before)
            || !typeof(IExecutionMiddlewareAsync).IsAssignableFrom(after))
        {
            throw new ArgumentException($"Both types must implement {nameof(IExecutionMiddlewareAsync)}");
        }

        var node = _middlewares.Find(after);

        if (node == null)
        {
            throw new InvalidOperationException($"Stage {after.Name} not found in the collection.");
        }

        _middlewares.AddBefore(node, before);
    }

    public void AddAfter<TBefore, TAfter>()
        where TBefore : IExecutionMiddlewareAsync
        where TAfter : IExecutionMiddlewareAsync
    {
        AddAfter(typeof(TBefore), typeof(TAfter));
    }

    public void AddAfter(Type before, Type after)
    {
        if (before == null)
        {
            throw new ArgumentNullException(nameof(before));
        }

        if (after == null)
        {
            throw new ArgumentNullException(nameof(after));
        }

        if (!typeof(IExecutionMiddlewareAsync).IsAssignableFrom(before)
            || !typeof(IExecutionMiddlewareAsync).IsAssignableFrom(after))
        {
            throw new ArgumentException($"Both types must implement {nameof(IExecutionMiddlewareAsync)}");
        }

        var node = _middlewares.Find(before);

        if (node == null)
        {
            throw new InvalidOperationException($"Stage {before.Name} not found in the collection.");
        }

        _middlewares.AddAfter(node, after);
    }

    public void Remove<T>()
        where T : IExecutionMiddlewareAsync
    {
        Remove(typeof(T));
    }

    public void Remove(Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        if (!typeof(IExecutionMiddlewareAsync).IsAssignableFrom(type))
        {
            throw new ArgumentException($"The type {type.Name} does not implement {nameof(IExecutionMiddlewareAsync)}");
        }

        var node = _middlewares.Find(type);

        if (node == null)
        {
            throw new InvalidOperationException($"Stage {type.Name} not found in the collection.");
        }

        _middlewares.Remove(node);
    }

    public IEnumerator<Type> GetEnumerator()
    {
        return _middlewares.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
