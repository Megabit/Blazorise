#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Represents a weekly view in a scheduler, allowing customization of the displayed week.
/// </summary>
/// <typeparam name="TItem">This type parameter is used to define the type of items that will be scheduled and displayed in the view.</typeparam>
public partial class SchedulerWeekView<TItem> : BaseSchedulerView<TItem>
{
    #region Methods

    protected override void OnInitialized()
    {
        Scheduler?.NotifySchedulerWeekView( this );

        base.OnInitialized();
    }

    #endregion

    #region Properties

    /// <summary>
    /// The first day of the week. Determines the first day of the week that is displayed in the scheduler.
    /// </summary>
    [Parameter] public DayOfWeek FirstDayOfWeek { get; set; } = DayOfWeek.Sunday;

    #endregion
}
