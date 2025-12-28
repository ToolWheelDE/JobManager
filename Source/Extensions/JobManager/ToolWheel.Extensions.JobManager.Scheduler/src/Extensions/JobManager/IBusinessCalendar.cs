using System;

namespace ToolWheel.Extensions.JobManager;

public interface IBusinessCalendar
{
    bool IsBusinessDay(DateTime localDate);
}
