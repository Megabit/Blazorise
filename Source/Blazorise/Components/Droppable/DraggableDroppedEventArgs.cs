﻿#region Using directives
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
        public DraggableDroppedEventArgs( TItem item, string dropZoneName )
        {
            Item = item;
            DropZoneName = dropZoneName;
        }

        /// <summary>
        /// Gets the dropped item during the transaction.
        /// </summary>
        public TItem Item { get; private set; }

        /// <summary>
        /// Gets the name of the zone where the transaction started.
        /// </summary>
        public string DropZoneName { get; private set; }
    }
}
