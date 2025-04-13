#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Scheduler.Components;

public partial class _SchedulerWeekView<TItem>
{
    #region Members

    #endregion

    #region Methods

    #endregion

    #region Properties

    [CascadingParameter] public Scheduler<TItem> Scheduler { get; set; }

    /// <summary>
    /// Gets or sets the date that is currently selected in the scheduler.
    /// </summary>
    [Parameter] public DateOnly SelectedDate { get; set; }

    /// <summary>
    /// Gets or sets the first day of the week.
    /// </summary>
    [Parameter] public DayOfWeek FirstDayOfWeek { get; set; }

    [Parameter] public TimeOnly? StartTime { get; set; }

    [Parameter] public TimeOnly? EndTime { get; set; }

    [Parameter] public TimeOnly? WorkDayStart { get; set; }

    [Parameter] public TimeOnly? WorkDayEnd { get; set; }

    [Parameter] public int SlotsPerCell { get; set; }

    [Parameter] public double HeaderCellHeight { get; set; }

    [Parameter] public double ItemCellHeight { get; set; }

    #endregion
}
