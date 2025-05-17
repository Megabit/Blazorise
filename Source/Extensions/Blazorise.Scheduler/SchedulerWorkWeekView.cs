#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Represents a scheduler view that displays only the working days of the week (typically Monday through Friday).
/// Allows customization of the first workday and cell height.
/// </summary>
/// <typeparam name="TItem">
/// The type of the items to be scheduled and displayed within the work week view.
/// </typeparam>
public partial class SchedulerWorkWeekView<TItem> : BaseSchedulerView<TItem>
{
    #region Methods

    /// <summary>
    /// Called when the component is initialized.
    /// Notifies the parent scheduler that the work week view is active.
    /// </summary>
    protected override void OnInitialized()
    {
        Scheduler?.NotifySchedulerWorkWeekView( this );

        base.OnInitialized();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the first day of the work week.
    /// Determines the starting day of the work week in the scheduler view.
    /// </summary>
    /// <remarks>
    /// The default value is <see cref="DayOfWeek.Monday"/>.
    /// </remarks>
    [Parameter] public DayOfWeek FirstDayOfWorkWeek { get; set; } = DayOfWeek.Monday;

    /// <summary>
    /// Gets or sets the height of each appointment cell in the layout.
    /// </summary>
    /// <remarks>
    /// The value is in pixels and affects vertical sizing of time slots.
    /// </remarks>
    [Parameter] public double ItemCellHeight { get; set; } = 60;

    #endregion
}
