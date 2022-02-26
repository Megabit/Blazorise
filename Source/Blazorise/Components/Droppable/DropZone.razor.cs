#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Wrapper component that are actiong as a drop area for the drop items.
    /// </summary>
    /// <typeparam name="TItem">Type of the draggable item.</typeparam>
    public partial class DropZone<TItem> : BaseComponent, IAsyncDisposable
    {
        #region Members

        /// <summary>
        /// Indicates that the drop operation is allowed.
        /// </summary>
        private bool dropAllowed;

        /// <summary>
        /// Indicates that the dragging item is entered into the dropzone.
        /// </summary>
        private bool itemOnDropZone;

        /// <summary>
        /// Indicates that the dragging operation is in process.
        /// </summary>
        private bool dragging;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            if ( ParentContainer != null )
            {
                ParentContainer.TransactionStarted += OnContainerTransactionStarted;
                ParentContainer.TransactionEnded += OnContainerTransactionEnded;
                ParentContainer.RefreshRequested += OnContainerRefreshRequested;
            }

            base.OnInitialized();
        }

        /// <inheritdoc/>
        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            if ( firstRender == true )
            {
                await JSModule.Initialize( ElementRef, ElementId );
            }

            await base.OnAfterRenderAsync( firstRender );
        }

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( "b-drop-zone" );
            builder.Append( "b-drop-zone-drag-block", TransactionInProgress && TransactionSourceZoneName != Name );
            builder.Append( DropAllowedClass ?? ParentContainer?.DropAllowedClass ?? "b-drop-zone-drop-allowed", TransactionInProgress && TransactionSourceZoneName != Name && dropAllowed && ( itemOnDropZone || GetApplyDropClassesOnDragStarted() ) );
            builder.Append( DropNotAllowedClass ?? ParentContainer?.DropNotAllowedClass ?? "b-drop-zone-drop-not-allowed", TransactionInProgress && TransactionSourceZoneName != Name && !dropAllowed && ( itemOnDropZone || GetApplyDropClassesOnDragStarted() ) );
            builder.Append( GetDraggingClass(), dragging );

            base.BuildClasses( builder );
        }

        /// <inheritdoc/>
        protected override async ValueTask DisposeAsync( bool disposing )
        {
            if ( disposing )
            {
                if ( Rendered )
                {
                    await JSModule.SafeDestroy( ElementRef, ElementId );
                }

                if ( ParentContainer != null )
                {
                    ParentContainer.TransactionStarted -= OnContainerTransactionStarted;
                    ParentContainer.TransactionEnded -= OnContainerTransactionEnded;
                    ParentContainer.RefreshRequested -= OnContainerRefreshRequested;
                }
            }

            await base.DisposeAsync( disposing );
        }

        private void OnContainerTransactionStarted( object sender, DraggableTransaction<TItem> e )
        {
            if ( GetApplyDropClassesOnDragStarted() )
            {
                var dropResult = ItemCanBeDropped();

                dropAllowed = dropResult.Item2;
            }

            InvokeAsync( StateHasChanged );
        }

        private void OnContainerTransactionEnded( object sender, EventArgs e )
        {
            itemOnDropZone = false;

            if ( GetApplyDropClassesOnDragStarted() )
            {
                dropAllowed = false;
            }

            DirtyClasses();

            InvokeAsync( StateHasChanged );
        }

        private void OnContainerRefreshRequested( object sender, EventArgs e )
        {
            DirtyClasses();

            InvokeAsync( StateHasChanged );
        }

        private void OnDragEnterHandler()
        {
            var (context, canBeDropped) = ItemCanBeDropped();

            if ( context == null )
            {
                return;
            }

            itemOnDropZone = true;
            dropAllowed = canBeDropped;

            DirtyClasses();
        }

        private void OnDragLeaveHandler()
        {
            var (context, _) = ItemCanBeDropped();

            if ( context == null )
            {
                return;
            }

            itemOnDropZone = false;

            DirtyClasses();
        }

        private async Task OnDropHandler()
        {
            var (context, canBeDropped) = ItemCanBeDropped();

            if ( context == null )
            {
                return;
            }

            itemOnDropZone = false;

            if ( !canBeDropped )
            {
                DirtyClasses();

                await ParentContainer.CancelTransaction();

                return;
            }

            await ParentContainer.CommitTransaction( Name );

            DirtyClasses();
        }

        private (TItem, bool) ItemCanBeDropped()
        {
            if ( !TransactionInProgress )
                return (default( TItem ), false);

            var item = ParentContainer.GetTransactionItem();

            if ( item == null )
                return (default( TItem ), false);

            var canBeDropped = true;

            if ( DropAllowed != null )
            {
                canBeDropped = DropAllowed( item );
            }
            else if ( ParentContainer.DropAllowed != null )
            {
                canBeDropped = ParentContainer.DropAllowed( item, Name );
            }

            return (item, canBeDropped);
        }

        private void OnDragStarted() => dragging = true;

        private void OnDragEnded() => dragging = false;

        private IEnumerable<TItem> GetItems()
        {
            Func<TItem, bool> predicate = ( item ) => ParentContainer.ItemsFilter( item, Name );

            if ( ItemsFilter != null )
            {
                predicate = ItemsFilter;
            }

            return ( ParentContainer?.Items ?? Enumerable.Empty<TItem>() ).Where( predicate ).ToArray();
        }

        private bool GetItemDisabled( TItem item )
        {
            var disabled = false;

            var predicate = ItemDisabled ?? ParentContainer?.ItemDisabled;

            if ( predicate != null )
            {
                disabled = predicate( item );
            }

            return disabled;
        }

        private string GetDraggingClass()
        {
            return DraggingClass ?? ParentContainer?.DraggingClass;
        }

        private string GetItemDisabledClass()
        {
            return DisabledClass ?? ParentContainer?.DisabledClass;
        }

        private string GetItemDraggingClass()
        {
            return ItemDraggingClass ?? ParentContainer?.ItemDraggingClass;
        }

        private bool GetApplyDropClassesOnDragStarted() => ( ApplyDropClassesOnDragStarted ?? ParentContainer?.ApplyDropClassesOnDragStarted ) ?? false;

        #endregion

        #region Properties

        /// <inheritdoc/>
        protected override bool ShouldAutoGenerateId => true;

        /// <summary>
        /// True if the drag transaction is in process.
        /// </summary>
        protected bool TransactionInProgress => ParentContainer?.TransactionInProgress == true;

        /// <summary>
        /// Gets the name of the dropzone that started the transaction.
        /// </summary>
        protected string TransactionSourceZoneName => ParentContainer?.TransactionSourceZoneName;

        /// <summary>
        /// Gets or sets the <see cref="IJSDragDropModule"/> instance.
        /// </summary>
        [Inject] public IJSDragDropModule JSModule { get; set; }

        /// <summary>
        /// Gets or sets the unique name of the dropzone.
        /// </summary>
        [Parameter] public string Name { get; set; }

        /// <summary>
        /// The method used to determine if the item belongs to the dropzone.
        /// </summary>
        [Parameter] public Func<TItem, bool> ItemsFilter { get; set; }

        /// <summary>
        /// The render method that is used to render the items withing the dropzone.
        /// </summary>
        [Parameter] public RenderFragment<TItem> ItemTemplate { get; set; }

        /// <summary>
        /// Determines if the item is allowed to be dropped to this zone.
        /// </summary>
        [Parameter] public Func<TItem, bool> DropAllowed { get; set; }

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
        [Parameter] public bool? ApplyDropClassesOnDragStarted { get; set; }

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
        /// Specifies the content to be rendered inside this <see cref="DropZone{TItem}"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Gets or sets the reference to the parent <see cref="DropContainer{TItem}"/> component.
        /// </summary>
        [CascadingParameter] protected DropContainer<TItem> ParentContainer { get; set; }

        #endregion
    }
}
