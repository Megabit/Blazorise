namespace Blazorise.Scheduler;

/// <summary>
/// Represents the view mode of the scheduler.
/// </summary>
public enum SchedulerView
{
    /// <summary>
    /// Represents a specific day, typically used in date-related contexts. It may include properties for day number and associated month.
    /// </summary>
    Day,

    /// <summary>
    /// Represents a week, typically used in date-related contexts. It may include properties for week number and associated month.
    /// </summary>
    Week,

    /// <summary>
    /// Represents the work week configuration or schedule. It defines the days and hours designated for work.
    /// </summary>
    WorkWeek,

    /// <summary>
    /// Represents the month view, typically used in date-related contexts. It may include properties for month number and associated year.
    /// </summary>
    Month,
}
