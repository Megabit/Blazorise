#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Represents a weekly view in a scheduler, allowing customization of the displayed week.
/// Displays a grid of days and time slots for the selected week.
/// </summary>
/// <typeparam name="TItem">
/// The type of the items to be scheduled and displayed in the view.
/// This allows the scheduler to work with any data model.
/// </typeparam>
public partial class SchedulerWeekView<TItem> : BaseSchedulerView<TItem>
{
    #region Methods

    /// <summary>
    /// Called when the component is initialized.
    /// Notifies the parent scheduler that the week view is active.
    /// </summary>
    protected override void OnInitialized()
    {
        Scheduler?.NotifySchedulerWeekView( this );

        base.OnInitialized();
    }

    #endregion

    #region Properties    

    /// <summary>
    /// Gets or sets the height of each appointment cell in the grid.
    /// </summary>
    /// <remarks>
    /// The value is a double representing the height in pixels.
    /// </remarks>
    [Parameter] public double ItemCellHeight { get; set; } = 60;

    #endregion
}
