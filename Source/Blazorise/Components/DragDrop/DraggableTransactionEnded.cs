#region Using directives
using System;
#endregion

namespace Blazorise;

/// <summary>
/// Defines the drag&amp;drop finished transaction.
/// </summary>
/// <typeparam name="TItem">Type of drag&amp;drop item.</typeparam>
public class DraggableTransactionEnded<TItem> : EventArgs
{
    /// <summary>
    /// A default <see cref="DraggableTransactionEnded{TItem}"/> constructor.
    /// </summary>
    /// <param name="transaction">Transaction that is finished.</param>
    public DraggableTransactionEnded( DraggableTransaction<TItem> transaction ) :
        this( string.Empty, false, transaction )
    {
    }

    /// <summary>
    /// A manual <see cref="DraggableTransactionEnded{TItem}"/> constructor.
    /// </summary>
    /// <param name="destinationDropZoneName">Name of the zone where the transaction has ended.</param>
    /// <param name="success">Flag that indicated if the transaction was successful.</param>
    /// <param name="transaction">Transaction that is finished.</param>
    public DraggableTransactionEnded( string destinationDropZoneName, bool success, DraggableTransaction<TItem> transaction )
    {
        Item = transaction.Item;
        Success = success;
        OriginDropZoneName = transaction.SourceZoneName;
        DestinationDropZoneName = destinationDropZoneName;
        OriginIndex = transaction.SourceIndex;
        DestinationIndex = transaction.Index;
    }

    /// <summary>
    /// Gets the item that is being dragged during the transaction.
    /// </summary>
    public TItem Item { get; }

    /// <summary>
    /// Gets the flag that indicated if the transaction was successful.
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Gets the name of the drop zone where the transaction has started.
    /// </summary>
    public string OriginDropZoneName { get; }

    /// <summary>
    /// Gets the name of the zone where the transaction has ended.
    /// </summary>
    public string DestinationDropZoneName { get; }

    /// <summary>
    /// Gets the index of the drop item where the transaction has started.
    /// </summary>
    public int OriginIndex { get; }

    /// <summary>
    /// Gets the index of the drop item where the transaction has ended.
    /// </summary>
    public int DestinationIndex { get; }
}