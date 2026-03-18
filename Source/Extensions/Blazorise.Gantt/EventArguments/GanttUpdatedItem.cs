#region Using directives
using System;
#endregion

namespace Blazorise.Gantt;

/// <summary>
/// Event arguments raised when an item is updated or removed.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public class GanttUpdatedItem<TItem> : EventArgs
{
    /// <summary>
    /// Creates a new instance of <see cref="GanttUpdatedItem{TItem}"/>.
    /// </summary>
    public GanttUpdatedItem( TItem oldItem, TItem newItem )
    {
        OldItem = oldItem;
        NewItem = newItem;
    }

    /// <summary>
    /// Previous item value.
    /// </summary>
    public TItem OldItem { get; }

    /// <summary>
    /// New item value.
    /// </summary>
    public TItem NewItem { get; }
}