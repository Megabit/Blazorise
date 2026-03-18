#region Using directives
using System;
#endregion

namespace Blazorise.Gantt;

/// <summary>
/// Event arguments raised when a new item is inserted.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public class GanttInsertedItem<TItem> : EventArgs
{
    /// <summary>
    /// Creates a new instance of <see cref="GanttInsertedItem{TItem}"/>.
    /// </summary>
    public GanttInsertedItem( TItem item )
    {
        Item = item;
    }

    /// <summary>
    /// Inserted item.
    /// </summary>
    public TItem Item { get; }
}