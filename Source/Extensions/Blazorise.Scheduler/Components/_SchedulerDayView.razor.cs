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
    #region Properties

    /// <summary>
    /// Gets or sets the parent <see cref="Scheduler{TItem}"/> component.
    /// </summary>
    [CascadingParameter] public Scheduler<TItem> Scheduler { get; set; }

    /// <summary>
    /// Gets or sets the selected date to display in the day view.
    /// </summary>
    [Parameter] public DateOnly SelectedDate { get; set; }

    /// <summary>
    /// Gets or sets the starting time of the day view.
    /// </summary>
    [Parameter] public TimeOnly StartTime { get; set; }

    /// <summary>
    /// Gets or sets the ending time of the day view.
    /// </summary>
    [Parameter] public TimeOnly EndTime { get; set; }

    /// <summary>
    /// Gets or sets the beginning of the working hours.
    /// </summary>
    [Parameter] public TimeOnly? WorkDayStart { get; set; }

    /// <summary>
    /// Gets or sets the end of the working hours.
    /// </summary>
    [Parameter] public TimeOnly? WorkDayEnd { get; set; }

    /// <summary>
    /// Gets or sets the number of time slots per hour.
    /// For example, 4 will create 15-minute slots.
    /// </summary>
    [Parameter] public int SlotsPerCell { get; set; }

    /// <summary>
    /// Gets or sets the height, in pixels, of each header cell.
    /// </summary>
    [Parameter] public double HeaderCellHeight { get; set; }

    /// <summary>
    /// Gets or sets the height, in pixels, of each item cell.
    /// </summary>
    [Parameter] public double ItemCellHeight { get; set; }

    #endregion
}
