#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Represents a monthly view in a scheduler, allowing customization of how the entire month is displayed,
/// including the size of day cells and the overall layout.
/// </summary>
/// <typeparam name="TItem">
/// The type of the items that will be scheduled and rendered in the month view.
/// </typeparam>
public partial class SchedulerMonthView<TItem> : BaseSchedulerView<TItem>
{
    #region Methods

    /// <summary>
    /// Called when the component is initialized.
    /// Notifies the parent scheduler that the month view is active and ready for rendering.
    /// </summary>
    protected override void OnInitialized()
    {
        Scheduler?.NotifySchedulerMonthView( this );

        base.OnInitialized();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the height of each day cell in the month view layout, measured in pixels.
    /// </summary>
    /// <remarks>
    /// This value controls the vertical space allocated to each day in the month grid.
    /// The default value is <c>100</c>.
    /// </remarks>
    [Parameter] public double ItemCellHeight { get; set; } = 100;

    /// <summary>
    /// Indicates whether to display week numbers in a calendar. Defaults to true.
    /// </summary>
    [Parameter] public bool ShowWeekNumbers { get; set; } = true;

    #endregion
}
