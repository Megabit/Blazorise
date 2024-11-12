#region Using directives
namespace Blazorise;
#endregion

/// <summary>
/// Represents an item in a drop zone along with its order.
/// </summary>
/// <typeparam name="TItem">The type of the item.</typeparam>
public class DropZoneOrderItem<TItem>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DropZoneOrderItem{TItem}"/> class with the specified item and order.
    /// </summary>
    /// <param name="item">The item in the drop zone.</param>
    /// <param name="order">The order of the item within the drop zone.</param>
    public DropZoneOrderItem( TItem item, int order )
    {
        Item = item;
        Order = order;
    }

    /// <summary>
    /// Gets the item in the drop zone.
    /// </summary>
    public TItem Item { get; }

    /// <summary>
    /// Gets the order of the item within the drop zone.
    /// </summary>
    public int Order { get; }
}