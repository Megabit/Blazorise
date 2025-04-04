#region Using directives
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise;

/// <summary>
/// Represents an ordered collection of items for a drop zone.
/// </summary>
/// <typeparam name="TItem">The type of items in the drop zone.</typeparam>
public class DropZoneOrder<TItem>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DropZoneOrder{TItem}"/> class with the specified ordered items.
    /// </summary>
    /// <param name="originDropZoneName">The name of the drop zone where the transaction has started.</param>
    /// <param name="destinationDropZoneName">The name of the zone where the transaction has ended.</param>
    /// <param name="orderedItems">The list of ordered items in the drop zone.</param>
    public DropZoneOrder( string originDropZoneName, string destinationDropZoneName, List<DropZoneOrderItem<TItem>> orderedItems )
    {
        OriginDropZoneName = originDropZoneName;
        DestinationDropZoneName = destinationDropZoneName;
        OrderedItems = orderedItems;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DropZoneOrder{TItem}"/> class with the specified dictionary of items and their orders.
    /// </summary>
    /// <param name="originDropZoneName">The name of the drop zone where the transaction has started.</param>
    /// <param name="destinationDropZoneName">The name of the zone where the transaction has ended.</param>
    /// <param name="orderedItems">A dictionary where the key is the item in the drop zone, and the value is its order.</param>
    internal DropZoneOrder( string originDropZoneName, string destinationDropZoneName, Dictionary<TItem, int> orderedItems )
    {
        OriginDropZoneName = originDropZoneName;
        DestinationDropZoneName = destinationDropZoneName;
        OrderedItems = orderedItems?.Select( x => new DropZoneOrderItem<TItem>( x.Key, x.Value ) )?.ToList();
    }

    /// <summary>
    /// Gets the name of the drop zone where the transaction has started.
    /// </summary>
    public string OriginDropZoneName { get; }

    /// <summary>
    /// Gets the name of the zone where the transaction has ended.
    /// </summary>
    public string DestinationDropZoneName { get; }

    /// <summary>
    /// Gets the list of ordered items in the drop zone. Each item includes the object and its assigned order.
    /// </summary>
    public List<DropZoneOrderItem<TItem>> OrderedItems { get; }
}
