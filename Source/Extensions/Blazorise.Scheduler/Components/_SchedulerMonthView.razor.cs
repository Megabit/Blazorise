﻿#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Scheduler.Components;

/// <summary>
/// Represents the month view layout for the <see cref="Scheduler{TItem}"/> component,
/// rendering a grid of days for the selected month.
/// </summary>
/// <typeparam name="TItem">The type of the scheduler item.</typeparam>
public partial class _SchedulerMonthView<TItem>
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
    /// Gets or sets the parent scheduler instance.
    /// </summary>
    [CascadingParameter] public Scheduler<TItem> Scheduler { get; set; }

    /// <summary>
    /// Gets or sets the date that is currently selected in the scheduler.
    /// </summary>
    [Parameter] public DateOnly SelectedDate { get; set; }

    /// <summary>
    /// Gets or sets the first day of the week.
    /// This affects the layout of the calendar grid (e.g., starting on Sunday or Monday).
    /// </summary>
    [Parameter] public DayOfWeek FirstDayOfWeek { get; set; }

    /// <summary>
    /// Gets or sets the earliest time of day that can be shown or used in the month view.
    /// </summary>
    [Parameter] public TimeOnly StartTime { get; set; }

    /// <summary>
    /// Gets or sets the latest time of day that can be shown or used in the month view.
    /// </summary>
    [Parameter] public TimeOnly EndTime { get; set; }

    /// <summary>
    /// Gets or sets the start of the working hours for visual styling (e.g., highlighting working time).
    /// </summary>
    [Parameter] public TimeOnly? WorkDayStart { get; set; }

    /// <summary>
    /// Gets or sets the end of the working hours for visual styling.
    /// </summary>
    [Parameter] public TimeOnly? WorkDayEnd { get; set; }

    /// <summary>
    /// Gets or sets the height, in pixels, of the header row (typically for day names).
    /// </summary>
    [Parameter] public double HeaderCellHeight { get; set; }

    /// <summary>
    /// Gets or sets the height, in pixels, of each cell used to display items in the calendar.
    /// </summary>
    [Parameter] public double ItemCellHeight { get; set; }

    /// <summary>
    /// Indicates whether to display week numbers in a calendar. Defaults to true.
    /// </summary>
    [Parameter] public bool ShowWeekNumbers { get; set; } = true;

    /// <summary>
    /// Specifies the maximum height for the view, allowing for a nullable double value. If not set, there is no height restriction.
    /// </summary>
    [Parameter] public double? ViewHeight { get; set; }

    #endregion
}
