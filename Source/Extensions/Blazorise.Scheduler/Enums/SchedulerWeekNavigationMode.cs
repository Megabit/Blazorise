namespace Blazorise.Scheduler;

/// <summary>
/// Defines options for navigating a week in a scheduler.
/// </summary>
public enum SchedulerWeekNavigationMode
{
    /// <summary>
    /// Represents the current day in a date context. It is used to determine the current day for weekly calculations.
    /// </summary>
    CurrentDay,

    /// <summary>
    /// Represents the first day of the week. It is used to determine the starting day for weekly calculations.
    /// </summary>
    FirstDayOfWeek,
}
