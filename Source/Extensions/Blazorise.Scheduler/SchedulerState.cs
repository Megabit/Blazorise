#region Using directives
using System;
using Blazorise.Infrastructure;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Manages the state and events for the scheduler component.
/// </summary>
public record SchedulerState
{
    /// <summary>
    /// Gets or sets the date that is currently selected in the scheduler. Defaults to today.
    /// </summary>
    public DateOnly SelectedDate { get; set; } = DateOnly.FromDateTime( DateTime.Today );

    /// <summary>
    /// Gets the event callback that is triggered when a request is made to navigate to the next day.
    /// </summary>
    internal EventCallbackSubscribable NextDayRequested { get; } = new();

    /// <summary>
    /// Gets the event callback that is triggered when a request is made to navigate to the previous day.
    /// </summary>
    internal EventCallbackSubscribable PrevDayRequested { get; } = new();

    /// <summary>
    /// Gets the event callback that is triggered when a request is made to navigate to today.
    /// </summary>
    internal EventCallbackSubscribable TodayRequested { get; } = new();

    /// <summary>
    /// Gets the event callback that is triggered when a request is made to navigate to the day view.
    /// </summary>
    internal EventCallbackSubscribable DayViewRequested { get; } = new();

    /// <summary>
    /// Gets the event callback that is triggered when a request is made to navigate to the week view.
    /// </summary>
    internal EventCallbackSubscribable WeekViewRequested { get; } = new();

    /// <summary>
    /// Gets the event callback that is triggered when a request is made to navigate to the work week view.
    /// </summary>
    internal EventCallbackSubscribable WorkWeekViewRequested { get; } = new();

    /// <summary>
    /// Gets the event callback that is triggered when a request is made to navigate to the month view.
    /// </summary>
    internal EventCallbackSubscribable MonthViewRequested { get; } = new();

    //internal object DraggedItem { get; set; }
}
