#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Scheduler.Components;

/// <summary>
/// Represents a week view in a scheduler component, allowing configuration of various display and time settings.
/// </summary>
/// <typeparam name="TItem">This type parameter is used to define the type of items that will be scheduled and displayed in the component.</typeparam>
public partial class _SchedulerWeekView<TItem>
{
    #region Properties

    /// <summary>
    /// Provides access to the parent <see cref="Scheduler{TItem}"/> component.
    /// </summary>
    [CascadingParameter] public Scheduler<TItem> Scheduler { get; set; }

    /// <summary>
    /// Gets or sets the date that is currently selected in the scheduler.
    /// </summary>
    [Parameter] public DateOnly SelectedDate { get; set; }

    /// <summary>
    /// Gets or sets the first day of the week to display in the week view.
    /// </summary>
    [Parameter] public DayOfWeek FirstDayOfWeek { get; set; }

    /// <summary>
    /// Gets or sets the optional starting time for the visible scheduler range.
    /// </summary>
    [Parameter] public TimeOnly? StartTime { get; set; }

    /// <summary>
    /// Gets or sets the optional ending time for the visible scheduler range.
    /// </summary>
    [Parameter] public TimeOnly? EndTime { get; set; }

    /// <summary>
    /// Gets or sets the optional start time for the workday hours.
    /// </summary>
    [Parameter] public TimeOnly? WorkDayStart { get; set; }

    /// <summary>
    /// Gets or sets the optional end time for the workday hours.
    /// </summary>
    [Parameter] public TimeOnly? WorkDayEnd { get; set; }

    /// <summary>
    /// Gets or sets the number of time slots within a single cell (hour).
    /// </summary>
    [Parameter] public int SlotsPerCell { get; set; }

    /// <summary>
    /// Gets or sets the height in pixels of the header cells.
    /// </summary>
    [Parameter] public double HeaderCellHeight { get; set; }

    /// <summary>
    /// Gets or sets the height in pixels of the item cells.
    /// </summary>
    [Parameter] public double ItemCellHeight { get; set; }

    #endregion
}
