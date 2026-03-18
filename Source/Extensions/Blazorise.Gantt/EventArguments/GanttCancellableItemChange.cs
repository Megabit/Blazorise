#region Using directives
using System.ComponentModel;
#endregion

namespace Blazorise.Gantt;

/// <summary>
/// Event arguments used to validate item insert or update operations.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public class GanttCancellableItemChange<TItem> : CancelEventArgs
{
    /// <summary>
    /// Creates a new instance of <see cref="GanttCancellableItemChange{TItem}"/>.
    /// </summary>
    public GanttCancellableItemChange( TItem oldItem, TItem newItem )
    {
        OldItem = oldItem;
        NewItem = newItem;
    }

    /// <summary>
    /// Original item value.
    /// </summary>
    public TItem OldItem { get; }

    /// <summary>
    /// New item value.
    /// </summary>
    public TItem NewItem { get; }
}