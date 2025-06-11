#region Using directives
namespace Blazorise.Scheduler;
#endregion

using Microsoft.AspNetCore.Components;

/// <summary>
/// Represents a view for scheduling events on a daily basis within a scheduler framework.
/// </summary>
/// <typeparam name="TItem">Represents the type of items that can be scheduled, such as events or tasks.</typeparam>
public partial class SchedulerDayView<TItem> : BaseSchedulerView<TItem>
{
    #region Members

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        Scheduler?.NotifySchedulerDayView( this );

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
