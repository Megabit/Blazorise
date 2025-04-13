#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Represents a base class for a scheduler view component, allowing customization of time settings and scheduling parameters.
/// </summary>
/// <typeparam name="TItem">Specifies the type of items that the scheduler will manage, enabling flexibility in the data being scheduled.</typeparam>
public class BaseSchedulerView<TItem> : ComponentBase
{
    #region Properties

    /// <summary>
    /// Gets or sets the scheduler component that the view belongs to.
    /// </summary>
    [CascadingParameter] public Scheduler<TItem> Scheduler { get; set; }

    /// <summary>
    /// Defines the first hour displayed in the view. Default is 00:00.
    /// </summary>
    [Parameter] public TimeOnly? StartTime { get; set; }

    /// <summary>
    /// Defines when the day ends.
    /// </summary>
    [Parameter] public TimeOnly? EndTime { get; set; }

    /// <summary>
    /// Represents the start time of a workday, using a nullable TimeOnly type. It allows for the specification of a workday's beginning time.
    /// </summary>
    [Parameter] public TimeOnly? WorkDayStart { get; set; }

    /// <summary>
    /// Represents the optional end time of a workday as a TimeOnly value. It can be null if not specified.
    /// </summary>
    [Parameter] public TimeOnly? WorkDayEnd { get; set; }

    /// <summary>
    /// Defines the number of slots available per cell, defaulting to 2. This parameter can be adjusted to change the cell configuration.
    /// </summary>
    [Parameter] public int SlotsPerCell { get; set; } = 2;

    /// <summary>
    /// Specifies the height of the header cell in pixels. The default value is set to 60.
    /// </summary>
    [Parameter] public double HeaderCellHeight { get; set; } = 60;

    /// <summary>
    /// The first day of the week. Determines the first day of the week that is displayed in the scheduler. Default is Sunday.
    /// </summary>
    [Parameter] public DayOfWeek FirstDayOfWeek { get; set; } = DayOfWeek.Sunday;

    #endregion
}
