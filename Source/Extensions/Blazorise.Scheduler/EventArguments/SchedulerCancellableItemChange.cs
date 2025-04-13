#region Using Directives
using System.ComponentModel;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Represents a change between two items, allowing for cancellation of the change if needed.
/// </summary>
/// <typeparam name="TItem">The type of the items being changed, which can be any data type relevant to the context.</typeparam>
public class SchedulerCancellableItemChange<TItem> : CancelEventArgs
{
    /// <summary>
    /// Represents a change between two items, allowing for cancellation of the change if needed.
    /// </summary>
    /// <param name="oldItem">The item that is being replaced or modified.</param>
    /// <param name="newItem">The item that will replace or modify the existing item.</param>
    public SchedulerCancellableItemChange( TItem oldItem, TItem newItem )
    {
        OldItem = oldItem;
        NewItem = newItem;
    }

    /// <summary>
    /// Represents the previous item in a collection or data structure. It is a read-only property.
    /// </summary>
    public TItem OldItem { get; }

    /// <summary>
    /// Represents a new item of type TItem. It provides read-only access to the new item.
    /// </summary>
    public TItem NewItem { get; }
}