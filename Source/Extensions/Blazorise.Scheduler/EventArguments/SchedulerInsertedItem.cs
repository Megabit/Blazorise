#region Using Directives
using System;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Represents an event argument that contains an item inserted into a scheduler.
/// </summary>
/// <typeparam name="TItem">The type of the item that has been inserted, allowing for flexibility in the type of data being handled.</typeparam>
public class SchedulerInsertedItem<TItem> : EventArgs
{
    /// <summary>
    /// Represents an item that has been inserted in the scheduler.
    /// </summary>
    /// <param name="item">Represents the current state of the item after the insertion.</param>
    public SchedulerInsertedItem( TItem item )
    {
        Item = item;
    }

    /// <summary>
    /// Represents an item of type TItem. It provides read-only access to the item.
    /// </summary>
    public TItem Item { get; }
}
