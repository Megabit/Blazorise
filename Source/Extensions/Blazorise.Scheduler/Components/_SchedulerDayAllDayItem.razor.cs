#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Scheduler.Components;

/// <summary>
/// Represents an all-day item in a scheduler, allowing interaction through click and drag events.
/// </summary>
/// <typeparam name="TItem">Represents the type of data displayed in the all-day slot, enabling customization of the item content.</typeparam>
public partial class _SchedulerDayAllDayItem<TItem>
{
    #region Methods

    /// <summary>
    /// Handles the click event on the all-day item and invokes the <see cref="Clicked"/> callback.
    /// </summary>
    protected Task OnSlotClicked()
    {
        if ( Clicked is null )
            return Task.CompletedTask;

        return Clicked.Invoke( Item );
    }

    /// <summary>
    /// Handles the drag start event for the all-day item.
    /// </summary>
    /// <param name="e">The drag event arguments.</param>
    /// <param name="item">The item being dragged.</param>
    protected Task OnItemDragStart( DragEventArgs e, TItem item )
    {
        return Scheduler.StartDrag( item, DragSection );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Returns a string indicating whether the Scheduler is draggable. It returns 'true' if draggable, otherwise 'false'.
    /// </summary>
    private string DraggableAttribute => Scheduler?.Editable == true && Scheduler?.UseInternalEditing == true && Scheduler?.Draggable == true ? "true" : "false";

    /// <summary>
    /// Provides access to the parent <see cref="Scheduler{TItem}"/> component.
    /// </summary>
    [CascadingParameter] public Scheduler<TItem> Scheduler { get; set; }

    /// <summary>
    /// Gets or sets the data item displayed in this all-day slot.
    /// </summary>
    [Parameter] public TItem Item { get; set; }

    /// <summary>
    /// Gets or sets whether the item overflows from the previous day.
    /// </summary>
    [Parameter] public bool OverflowingFromStart { get; set; }

    /// <summary>
    /// Gets or sets whether the item overflows into the next day.
    /// </summary>
    [Parameter] public bool OverflowingOnEnd { get; set; }

    /// <summary>
    /// Callback triggered when the item is clicked.
    /// </summary>
    [Parameter] public Func<TItem, Task> Clicked { get; set; }

    /// <summary>
    /// Gets or sets the drag area to be used when initiating drag operations.
    /// </summary>
    [Parameter] public SchedulerSection DragSection { get; set; }

    #endregion
}
