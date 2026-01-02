using System;
using System.Collections;
using System.Collections.Generic;
using ToolWheel.Extensions.JobManager.Pipeline;

namespace ToolWheel.Extensions.JobManager.Configuration;

public class ExecutionConditionCollection : ICollection<Type>
{
    private readonly List<Type> _conditions = new List<Type>();

    public int Count
    {
        get { return _conditions.Count; }
    }

    public bool IsReadOnly
    {
        get
        {
            return false;
        }
    }

    public void Add<T>()
        where T : IExecutionCondition
    {
        Add(typeof(T));
    }

    public void Add(Type item)
    {
        _conditions.Add(item);
    }

    public void Clear()
    {
        _conditions.Clear();
    }

    public bool Contains(Type item)
    {
        return _conditions.Contains(item);
    }

    public void CopyTo(Type[] array, int arrayIndex)
    {
        _conditions.CopyTo(array, arrayIndex);
    }

    public IEnumerator<Type> GetEnumerator()
    {
        return _conditions.GetEnumerator();
    }

    public bool Remove(Type item)
    {
        return _conditions.Remove(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
