namespace Blazorise.Scheduler;

/// <summary>
/// Defines areas in a scheduler where drag-and-drop actions can occur.
/// </summary>
public enum SchedulerDragArea
{
    /// <summary>
    /// No member is defined.
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
