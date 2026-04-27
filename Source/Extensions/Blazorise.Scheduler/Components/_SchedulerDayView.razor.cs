#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Scheduler.Components;

/// <summary>
/// Represents the day view layout of the <see cref="Scheduler{TItem}"/> component,
/// displaying the time slots for a single day.
/// </summary>
/// <typeparam name="TItem">The type of the scheduler item.</typeparam>
public partial class _SchedulerDayView<TItem>
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
    /// Specifies the parent <see cref="Scheduler{TItem}"/> component.
    /// </summary>
    [CascadingParameter] public Scheduler<TItem> Scheduler { get; set; }

    /// <summary>
    /// Specifies the selected date to display in the day view.
    /// </summary>
    [Parameter] public DateOnly SelectedDate { get; set; }

    /// <summary>
    /// Specifies the starting time of the day view.
    /// </summary>
    [Parameter] public TimeOnly StartTime { get; set; }

    /// <summary>
    /// Specifies the ending time of the day view.
    /// </summary>
    [Parameter] public TimeOnly EndTime { get; set; }

    /// <summary>
    /// Specifies the beginning of the working hours.
    /// </summary>
    [Parameter] public TimeOnly? WorkDayStart { get; set; }

    /// <summary>
    /// Specifies the end of the working hours.
    /// </summary>
    [Parameter] public TimeOnly? WorkDayEnd { get; set; }

    /// <summary>
    /// Specifies the number of time slots per hour.
    /// For example, 4 will create 15-minute slots.
    /// </summary>
    [Parameter] public int SlotsPerCell { get; set; }

    /// <summary>
    /// Specifies the height, in pixels, of each header cell.
    /// </summary>
    [Parameter] public double HeaderCellHeight { get; set; }

    /// <summary>
    /// Specifies the height, in pixels, of each item cell.
    /// </summary>
    [Parameter] public double ItemCellHeight { get; set; }

    /// <summary>
    /// Specifies the maximum height for the view, allowing for a nullable double value. If not set, there is no height restriction.
    /// </summary>
    [Parameter] public double? ViewHeight { get; set; }

    #endregion
}
