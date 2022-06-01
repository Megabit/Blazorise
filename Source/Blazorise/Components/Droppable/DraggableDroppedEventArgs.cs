#region Using directives
using System;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Defines the draggable item that is being dropped onto the dropzone.
    /// </summary>
    /// <typeparam name="TItem">Type of dropped item.</typeparam>
    public class DraggableDroppedEventArgs<TItem> : EventArgs
    {
        /// <summary>
        /// A default <see cref="DraggableDroppedEventArgs{TItem}"/> constructor.
        /// </summary>
        /// <param name="item">The dropped item during the transaction.</param>
        /// <param name="dropZoneName">Name of the zone where the transaction started.</param>
        /// <param name="indexInZone">The index of the item within the dropzone.</param>
        public DraggableDroppedEventArgs( TItem item, string dropZoneName, int indexInZone )
        {
            Item = item;
            DropZoneName = dropZoneName;
            IndexInZone = indexInZone;
        }

        /// <summary>
        /// Gets the dropped item during the transaction.
        /// </summary>
        public TItem Item { get; private set; }

        /// <summary>
        /// Gets the name of the zone where the transaction started.
        /// </summary>
        public string DropZoneName { get; private set; }

        /// <summary>
        /// The index of the item within the dropzone.
        /// </summary>
        public int IndexInZone { get; private set; }
    }
}
