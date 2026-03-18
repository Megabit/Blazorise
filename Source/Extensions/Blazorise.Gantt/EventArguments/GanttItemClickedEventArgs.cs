#region Using directives
using System;
#endregion

namespace Blazorise.Gantt;

/// <summary>
/// Event arguments raised when an item is clicked.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public class GanttItemClickedEventArgs<TItem> : EventArgs
{
    /// <summary>
    /// Creates a new instance of <see cref="GanttItemClickedEventArgs{TItem}"/>.
    /// </summary>
    public GanttItemClickedEventArgs( TItem item )
    {
        Item = item;
    }

    /// <summary>
    /// Clicked item.
    /// </summary>
    public TItem Item { get; }
}