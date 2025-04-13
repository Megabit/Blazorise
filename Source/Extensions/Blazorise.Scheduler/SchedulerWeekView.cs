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
    /// Specifies the height of a cell in a layout. The value is a double representing the height in pixels.
    /// </summary>
    [Parameter] public double ItemCellHeight { get; set; } = 60;

    #endregion
}
