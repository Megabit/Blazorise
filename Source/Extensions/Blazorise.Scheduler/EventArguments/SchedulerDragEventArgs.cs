#region Using directives
using System;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Represents the context of a drag event in the <see cref="Scheduler{TItem}"/>.
/// </summary>
public class SchedulerDragEventArgs<TItem> : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SchedulerDragEventArgs{TItem}"/> class with a specified item.
    /// </summary>
    /// <param name="item">The object that represents the item being dragged in the scheduler.</param>
    public SchedulerDragEventArgs( TItem item )
    {
        Item = item;
    }

    /// <summary>
    /// Gets or sets the item being dragged.
    /// </summary>
    public TItem Item { get; }
}