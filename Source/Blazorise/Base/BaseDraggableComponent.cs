#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Base component for all the components that needs to have drag and drop support.
    /// </summary>
    public abstract class BaseDraggableComponent : BaseComponent
    {
        #region Methods

        /// <summary>
        /// Event handler for <see cref="Drag"/> event callback.
        /// </summary>
        /// <param name="eventArgs">Supplies information about an drag event that is being raised.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual Task OnDragHandler( DragEventArgs eventArgs )
        {
            return Drag.InvokeAsync( eventArgs );
        }

        /// <summary>
        /// Event handler for <see cref="DragEnd"/> event callback.
        /// </summary>
        /// <param name="eventArgs">Supplies information about an drag event that is being raised.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual Task OnDragEndHandler( DragEventArgs eventArgs )
        {
            return DragEnd.InvokeAsync( eventArgs );
        }

        /// <summary>
        /// Event handler for <see cref="DragEnter"/> event callback.
        /// </summary>
        /// <param name="eventArgs">Supplies information about an drag event that is being raised.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual Task OnDragEnterHandler( DragEventArgs eventArgs )
        {
            return DragEnter.InvokeAsync( eventArgs );
        }

        /// <summary>
        /// Event handler for <see cref="DragLeave"/> event callback.
        /// </summary>
        /// <param name="eventArgs">Supplies information about an drag event that is being raised.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual Task OnDragLeaveHandler( DragEventArgs eventArgs )
        {
            return DragLeave.InvokeAsync( eventArgs );
        }

        /// <summary>
        /// Event handler for <see cref="DragOver"/> event callback.
        /// </summary>
        /// <param name="eventArgs">Supplies information about an drag event that is being raised.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual Task OnDragOverHandler( DragEventArgs eventArgs )
        {
            return DragOver.InvokeAsync( eventArgs );
        }

        /// <summary>
        /// Event handler for <see cref="DragStart"/> event callback.
        /// </summary>
        /// <param name="eventArgs">Supplies information about an drag event that is being raised.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual Task OnDragStartHandler( DragEventArgs eventArgs )
        {
            return DragStart.InvokeAsync( eventArgs );
        }

        /// <summary>
        /// Event handler for <see cref="Drop"/> event callback.
        /// </summary>
        /// <param name="eventArgs">Supplies information about an drag event that is being raised.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual Task OnDropHandler( DragEventArgs eventArgs )
        {
            return Drop.InvokeAsync( eventArgs );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether the element can be dragged.
        /// </summary>
        [Parameter] public bool Draggable { get; set; }

        /// <summary>
        /// The drag event is fired every few hundred milliseconds as an element or text selection is being dragged by the user.
        /// </summary>
        [Parameter] public EventCallback<DragEventArgs> Drag { get; set; }

        /// <summary>
        /// Used to prevent the default action for an <see cref="Drag"/> event.
        /// </summary>
        [Parameter] public bool DragPreventDefault { get; set; }

        /// <summary>
        /// The dragend event is fired when a drag operation is being ended (by releasing a mouse button or hitting the escape key).
        /// </summary>
        [Parameter] public EventCallback<DragEventArgs> DragEnd { get; set; }

        /// <summary>
        /// Used to prevent the default action for an <see cref="DragEnd"/> event.
        /// </summary>
        [Parameter] public bool DragEndPreventDefault { get; set; }

        /// <summary>
        /// The dragenter event is fired when a dragged element or text selection enters a valid drop target.
        /// </summary>
        [Parameter] public EventCallback<DragEventArgs> DragEnter { get; set; }

        /// <summary>
        /// Used to prevent the default action for an <see cref="DragEnter"/> event.
        /// </summary>
        [Parameter] public bool DragEnterPreventDefault { get; set; }

        /// <summary>
        /// The dragleave event is fired when a dragged element or text selection leaves a valid drop target.
        /// </summary>
        [Parameter] public EventCallback<DragEventArgs> DragLeave { get; set; }

        /// <summary>
        /// Used to prevent the default action for an <see cref="DragLeave"/> event.
        /// </summary>
        [Parameter] public bool DragLeavePreventDefault { get; set; }

        /// <summary>
        /// The dragover event is fired when an element or text selection is being dragged over a valid drop target (every few hundred milliseconds).
        /// </summary>
        [Parameter] public EventCallback<DragEventArgs> DragOver { get; set; }

        /// <summary>
        /// Used to prevent the default action for an <see cref="DragOver"/> event.
        /// </summary>
        [Parameter] public bool DragOverPreventDefault { get; set; }

        /// <summary>
        /// The dragstart event is fired when the user starts dragging an element or text selection.
        /// </summary>
        [Parameter] public EventCallback<DragEventArgs> DragStart { get; set; }

        /// <summary>
        /// Used to prevent the default action for an <see cref="DragStart"/> event.
        /// </summary>
        [Parameter] public bool DragStartPreventDefault { get; set; }

        /// <summary>
        /// The drop event is fired when an element or text selection is dropped on a valid drop target.
        /// </summary>
        [Parameter] public EventCallback<DragEventArgs> Drop { get; set; }

        /// <summary>
        /// Used to prevent the default action for an <see cref="Drop"/> event.
        /// </summary>
        [Parameter] public bool DropPreventDefault { get; set; }

        #endregion
    }
}
