namespace Blazorise.Scheduler;

/// <summary>
/// Defines areas in a scheduler that can be customized or styled.
/// </summary>
public enum SchedulerSection
{
    /// <summary>
    /// Represents a section of the scheduler that is not defined or does not have a specific purpose.
    /// </summary>
    None,

    /// <summary>
    /// Represents a header that spans the entire day in a calendar view.
    /// </summary>
    AllDayHeader,

    /// <summary>
    /// Represents a view for displaying a month in a calendar.
    /// </summary>
    MonthView,

    /// <summary>
    /// Represents a view that displays a week of events or schedules.
    /// </summary>
    WeekView,

    /// <summary>
    /// Represents a view for displaying a single day in a calendar.
    /// </summary>
    DayView,
}
