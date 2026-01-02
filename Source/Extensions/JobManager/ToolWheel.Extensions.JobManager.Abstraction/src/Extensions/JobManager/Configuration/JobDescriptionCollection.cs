using System.Collections;
using System.Collections.Generic;

namespace ToolWheel.Extensions.JobManager.Configuration;

public class JobDescriptionCollection : ICollection<JobDescription>
{
    private readonly HashSet<JobDescription> _jobDescriptions = new();

    public int Count { get => _jobDescriptions.Count; }

    public bool IsReadOnly { get => false; }

    public void Add(JobDescription item)
    {
        _jobDescriptions.Add(item);
    }

    public void AddRange(IEnumerable<JobDescription> items)
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

    public bool Contains(JobDescription item)
    {
        return _jobDescriptions.Contains(item);
    }

    public void CopyTo(JobDescription[] array, int arrayIndex)
    {
        _jobDescriptions.CopyTo(array, arrayIndex);
    }

    public IEnumerator<JobDescription> GetEnumerator()
    {
        return _jobDescriptions.GetEnumerator();
    }

    public bool Remove(JobDescription item)
    {
        return _jobDescriptions.Remove(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
