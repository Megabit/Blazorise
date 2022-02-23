#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Wrapper component for the draggable items and dropzones.
    /// </summary>
    /// <typeparam name="TItem">Type of the draggable item.</typeparam>
    public partial class DropContainer<TItem> : BaseComponent
    {
        #region Members

        private DraggableTransaction<TItem> transaction;

        /// <summary>
        /// An event that occurs after the drag transaction has started.
        /// </summary>
        public event EventHandler<DraggableTransaction<TItem>> TransactionStarted;

        /// <summary>
        /// An event that occurs after the drag transaction has ended.
        /// </summary>
        public event EventHandler TransactionEnded;

        /// <summary>
        /// An event that occurs after the refresh has been requested.
        /// </summary>
        public event EventHandler RefreshRequested;

        #endregion

        #region Methods

        /// <summary>
        /// Starts the new drag &amp; drop transaction for the specified item and dropzone.
        /// </summary>
        /// <param name="item">Item that is being dragged.</param>
        /// <param name="sourceZoneName">Dropzone name that is the source of the drag operation.</param>
        /// <param name="commited">Callback that will be called after the successful transaction.</param>
        /// <param name="canceled">Callback that will be called when the transaction has been cancelled.</param>
        public void StartTransaction( TItem item, string sourceZoneName, Func<Task> commited, Func<Task> canceled )
        {
            transaction = new DraggableTransaction<TItem>( item, sourceZoneName, commited, canceled );

            TransactionStarted?.Invoke( this, transaction );
        }

        /// <summary>
        /// Commits the drag &amp; drop transaction.
        /// </summary>
        /// <param name="dropZoneName">Dropzone name that is the source of the drag operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task CommitTransaction( string dropZoneName )
        {
            await transaction.Commit();

            await ItemDropped.InvokeAsync( new DraggableDroppedEventArgs<TItem>( transaction.Item, dropZoneName ) );

            TransactionEnded?.Invoke( this, EventArgs.Empty );

            transaction = null;
        }

        /// <summary>
        /// Cancels the drag &amp; drop transaction.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task CancelTransaction()
        {
            await transaction.Cancel();

            TransactionEnded?.Invoke( this, EventArgs.Empty );

            transaction = null;
        }

        /// <summary>
        /// Gets the item that is currently in the transaction.
        /// </summary>
        /// <returns>Current transaction item.</returns>
        public TItem GetTransactionItem()
            => transaction.Item;

        /// <summary>
        /// Request the refresh of the drag container.
        /// </summary>
        public void Refresh()
            => RefreshRequested?.Invoke( this, EventArgs.Empty );

        #endregion

        #region Properties

        /// <summary>
        /// True if the drag transaction is in process.
        /// </summary>
        public bool TransactionInProgress
            => transaction != null;

        /// <summary>
        /// Gets the name of the dropzone that started the transaction.
        /// </summary>
        public string TransactionSourceZoneName
            => transaction?.SourceZoneName;

        /// <summary>
        /// Items that are used for the drag&amp;drop withing the container.
        /// </summary>
        [Parameter] public IEnumerable<TItem> Items { get; set; }

        /// <summary>
        /// The method used to determine if the item belongs to the dropzone.
        /// </summary>
        [Parameter] public Func<TItem, string, bool> ItemsFilter { get; set; }

        /// <summary>
        /// The render method that is used to render the items withing the dropzone.
        /// </summary>
        [Parameter] public RenderFragment<TItem> ItemTemplate { get; set; }

        /// <summary>
        /// Callback that indicates that an item has been dropped on a drop zone. Should be used to update the "status" of the data item.
        /// </summary>
        [Parameter] public EventCallback<DraggableDroppedEventArgs<TItem>> ItemDropped { get; set; }

        /// <summary>
        /// Determines if the item is allowed to be dropped to the specified zone.
        /// </summary>
        [Parameter] public Func<TItem, string, bool> DropAllowed { get; set; }

        /// <summary>
        /// Classname that is applied if dropping to the current zone is allowed.
        /// </summary>
        [Parameter] public string DropAllowedClass { get; set; }

        /// <summary>
        /// Classname that is applied if dropping to the current zone is not allowed.
        /// </summary>
        [Parameter] public string DropNotAllowedClass { get; set; }

        /// <summary>
        /// When true, <see cref="DropAllowedClass"/> or <see cref="DropNotAllowedClass"/> drop classes are applied as soon as a transaction has started.
        /// </summary>
        [Parameter] public bool ApplyDropClassesOnDragStarted { get; set; }

        /// <summary>
        /// Determines if the item is disabled for dragging and dropping.
        /// </summary>
        [Parameter] public Func<TItem, bool> ItemDisabled { get; set; }

        /// <summary>
        /// Classname that is applied to the dropzone if the result of <see cref="ItemDisabled"/> is false.
        /// </summary>
        [Parameter] public string DisabledClass { get; set; }

        /// <summary>
        /// Classname that is applied to the dropzone when the drag operation has started.
        /// </summary>
        [Parameter] public string DraggingClass { get; set; }

        /// <summary>
        /// Classname that is applied to the drag item when it is being dragged.
        /// </summary>
        [Parameter] public string ItemDraggingClass { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="DropContainer{TItem}"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
