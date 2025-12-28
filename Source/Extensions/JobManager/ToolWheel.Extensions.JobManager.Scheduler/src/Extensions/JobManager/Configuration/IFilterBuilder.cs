using System;
using ToolWheel.Extensions.JobManager.Filter;

namespace ToolWheel.Extensions.JobManager.Configuration;

public interface IFilterBuilder
{
    /// <summary>
    /// Fügt einen einfachen Filter in die aktuelle Gruppe ein (Default: AND-Gruppe).
    /// </summary>
    IFilterBuilder Add(IJobSchedulerFilter filter);

    /// <summary>
    /// Fügt eine AND-Gruppe hinzu (verschachtelbar).
    /// </summary>
    IFilterBuilder And(Action<IFilterBuilder> group);

    /// <summary>
    /// Fügt eine OR-Gruppe hinzu (verschachtelbar).
    /// </summary>
    IFilterBuilder Or(Action<IFilterBuilder> group);

    /// <summary>
    /// Fügt eine NOT-Gruppe hinzu (verschachtelbar).
    /// </summary>
    IFilterBuilder Not(Action<IFilterBuilder> group);

    /// <summary>
    /// Fügt einen Filter hinzu und verknüpft ihn mit der aktuellen Gruppe per AND.
    /// </summary>
    IFilterBuilder And(IJobSchedulerFilter filter);

    /// <summary>
    /// Fügt einen Filter hinzu und verknüpft ihn mit der aktuellen Gruppe per OR.
    /// </summary>
    IFilterBuilder Or(IJobSchedulerFilter filter);

    /// <summary>
    /// Finalisiert diese Filterdefinition zu einem IJobSchedulerFilter.
    /// Normalerweise nur intern vom JobScheduleBuilder aufgerufen.
    /// </summary>
    IJobSchedulerFilter Build();
}
