#region Using Directives
using System;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Represents an event that occurs when an item in the scheduler is updated, containing the old and new item states.
/// </summary>
/// <typeparam name="TItem">Represents the type of the item being updated in the scheduler.</typeparam>
public class SchedulerUpdatedItem<TItem> : EventArgs
{
    /// <summary>
    /// Represents an item that has been updated in the scheduler, capturing both the previous and current state.
    /// </summary>
    /// <param name="oldItem">Represents the previous state of the item before the update.</param>
    /// <param name="newItem">Represents the current state of the item after the update.</param>
    public SchedulerUpdatedItem( TItem oldItem, TItem newItem )
    {
        OldItem = oldItem;
        NewItem = newItem;
    }

    /// <summary>
    /// Represents the previous item in a collection or data structure. It is a read-only property.
    /// </summary>
    public TItem OldItem { get; }

    /// <summary>
    /// Represents a new item of type TItem. It provides read-only access to the new item instance.
    /// </summary>
    public TItem NewItem { get; }
}
