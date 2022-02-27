﻿#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Internal component that represents the drag item used by the <see cref="DropZone{TItem}"/>.
    /// </summary>
    /// <typeparam name="TItem">Datatype of the item being dragged.</typeparam>
    public partial class _Draggable<TItem> : BaseComponent
    {
        #region Members

        private bool disabled;

        private bool dragging;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( "b-drop-zone-draggable" );
            builder.Append( DisabledClass, Disabled );
            builder.Append( DraggingClass, dragging );

            base.BuildClasses( builder );
        }

        private async Task OnDragStartHandler()
        {
            if ( ParentContainer == null )
                return;

            dragging = true;

            ParentContainer.StartTransaction( Item, ZoneName, OnDroppedSucceeded, OnDroppedCanceled );

            await DragStarted.InvokeAsync();
        }

        private async Task OnDragEndHandler( DragEventArgs e )
        {
            if ( dragging )
            {
                dragging = false;

                await ParentContainer?.CancelTransaction();
            }
            else
            {
                await DragEnded.InvokeAsync( Item );
            }
        }

        private async Task OnDroppedSucceeded()
        {
            dragging = false;

            await DragEnded.InvokeAsync( Item );

            await InvokeAsync( StateHasChanged );
        }

        private async Task OnDroppedCanceled()
        {
            dragging = false;

            await DragEnded.InvokeAsync( Item );

            await InvokeAsync( StateHasChanged );
        }

        #endregion

        #region Properties

        /// <summary>
        /// The dropzone name this this draggable belongs to.
        /// </summary>
        [Parameter] public string ZoneName { get; set; }

        /// <summary>
        /// The data that is represented by this item.
        /// </summary>
        [Parameter] public TItem Item { get; set; }

        /// <summary>
        /// An event that is raised when a drag operation has started.
        /// </summary>
        [Parameter] public EventCallback<TItem> DragStarted { get; set; }

        /// <summary>
        /// An event that is raised when a drag operation has ended.
        /// </summary>
        [Parameter] public EventCallback<TItem> DragEnded { get; set; }

        /// <summary>
        /// If true, the draggable item canot be dragged.
        /// </summary>
        [Parameter]
        public bool Disabled
        {
            get => disabled; set
            {
                disabled = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// The classname that is applied when <see cref="Disabled"/> is set to true.
        /// </summary>
        [Parameter] public string DisabledClass { get; set; }

        /// <summary>
        /// The classname that is applied when a dragging operation is in progress.
        /// </summary>
        [Parameter] public string DraggingClass { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="_Draggable{TItem}"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Gets or sets the reference to the parent <see cref="DropContainer{TItem}"/> component.
        /// </summary>
        [CascadingParameter] protected DropContainer<TItem> ParentContainer { get; set; }

        #endregion
    }
}
