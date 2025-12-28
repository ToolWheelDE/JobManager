using System.Collections;
using System.Collections.Generic;

namespace ToolWheel.Extensions.JobManager.Configuration;

public class JobGroupDescriptionCollection : ICollection<JobGroupDescription>
{
    private readonly HashSet<JobGroupDescription> _jobDescriptions = new();

    public int Count { get => _jobDescriptions.Count; }

    public bool IsReadOnly { get => false; }

    public void Add(JobGroupDescription item)
    {
        _jobDescriptions.Add(item);
    }

    public void AddRange(IEnumerable<JobGroupDescription> items)
    {
        foreach (var item in items)
        {
            _jobDescriptions.Add(item);
        }
    }

    public void Clear()
    {
        _jobDescriptions.Clear();
    }

    public bool Contains(JobGroupDescription item)
    {
        return _jobDescriptions.Contains(item);
    }

    public void CopyTo(JobGroupDescription[] array, int arrayIndex)
    {
        _jobDescriptions.CopyTo(array, arrayIndex);
    }

    public IEnumerator<JobGroupDescription> GetEnumerator()
    {
        return _jobDescriptions.GetEnumerator();
    }

    public bool Remove(JobGroupDescription item)
    {
        return _jobDescriptions.Remove(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
