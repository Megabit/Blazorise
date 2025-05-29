#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Scheduler.Components;

/// <summary>
/// Represents the "Work Week" view of the <see cref="Scheduler{TItem}"/> component,
/// displaying only the defined working days (e.g., Monday to Friday).
/// </summary>
/// <typeparam name="TItem">The type of the scheduler item.</typeparam>
public partial class _SchedulerWorkWeekView<TItem>
{
    #region Methods

    /// <summary>
    /// Generates a CSS style string for the view.
    /// </summary>
    /// <returns>Returns a string containing the CSS style or null.</returns>
    protected string GetViewStyle()
    {
        if ( ViewHeight is not null )
            return $"height: {ViewHeight}px; overflow-y: auto;";

        return null;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the parent <see cref="Scheduler{TItem}"/> component.
    /// </summary>
    [CascadingParameter] public Scheduler<TItem> Scheduler { get; set; }

    /// <summary>
    /// Gets or sets the date that is currently selected in the scheduler.
    /// </summary>
    [Parameter] public DateOnly SelectedDate { get; set; }

    /// <summary>
    /// Gets or sets the first day of the work week (e.g., Monday).
    /// </summary>
    [Parameter] public DayOfWeek FirstDayOfWorkWeek { get; set; }

    /// <summary>
    /// Gets or sets the start time of the scheduler view (e.g., 08:00).
    /// </summary>
    [Parameter] public TimeOnly StartTime { get; set; }

    /// <summary>
    /// Gets or sets the end time of the scheduler view (e.g., 17:00).
    /// </summary>
    [Parameter] public TimeOnly EndTime { get; set; }

    /// <summary>
    /// Gets or sets the start of the working hours (for highlighting work hours).
    /// </summary>
    [Parameter] public TimeOnly? WorkDayStart { get; set; }

    /// <summary>
    /// Gets or sets the end of the working hours (for highlighting work hours).
    /// </summary>
    [Parameter] public TimeOnly? WorkDayEnd { get; set; }

    /// <summary>
    /// Gets or sets how many time slots should be displayed per hour.
    /// For example, 4 would show 15-minute intervals.
    /// </summary>
    [Parameter] public int SlotsPerCell { get; set; }

    /// <summary>
    /// Gets or sets the height of each header cell (typically for hours).
    /// </summary>
    [Parameter] public double HeaderCellHeight { get; set; }

    /// <summary>
    /// Gets or sets the height of each item cell (used to display appointments).
    /// </summary>
    [Parameter] public double ItemCellHeight { get; set; }

    /// <summary>
    /// Specifies the maximum height for the view, allowing for a nullable double value. If not set, there is no height restriction.
    /// </summary>
    [Parameter] public double? ViewHeight { get; set; }

    #endregion
}
