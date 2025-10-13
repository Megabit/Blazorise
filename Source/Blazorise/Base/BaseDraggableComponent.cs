#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
#endregion

namespace Blazorise;

/// <summary>
/// Base component for all the components that needs to have drag and drop support.
/// </summary>
public abstract class BaseDraggableComponent : BaseComponent, IDisposable, IAsyncDisposable
{
    #region Members

    /// <summary>
    /// Object reference that can be accessed through the JSInterop.
    /// </summary>
    private DotNetObjectReference<BaseDraggableComponent> dotNetObjectRef;

    /// <summary>
    /// Is <c>true</c> if the component needs to clean up the JS ThrottleDragEvent handler on <see cref="Dispose"/> or <see cref="DisposeAsync"/>.
    /// </summary>
    private bool jsEventsInitialized;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override async Task OnFirstAfterRenderAsync()
    {
        if ( Draggable && ( Drag.HasDelegate || DragOver.HasDelegate ) )
        {
            dotNetObjectRef = CreateDotNetObjectRef( this );
            await JSDragDropModule.InitializeThrottledDragEvents( ElementRef, ElementId, dotNetObjectRef );
            jsEventsInitialized = true;
        }
        await base.OnFirstAfterRenderAsync();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            if ( jsEventsInitialized )
            {
                JSDragDropModule.DestroyThrottledDragEvents( ElementRef, ElementId );
            }

            DisposeDotNetObjectRef( dotNetObjectRef );
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
        {
            if ( jsEventsInitialized )
            {
                await JSDragDropModule.DestroyThrottledDragEvents( ElementRef, ElementId );
            }

            DisposeDotNetObjectRef( dotNetObjectRef );
        }

        await base.DisposeAsync( disposing );
    }

    /// <summary>
    /// Event handler for <see cref="Drag"/> event callback.
    /// </summary>
    /// <param name="eventArgs">Supplies information about an drag event that is being raised.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public virtual Task OnDragHandler( DragEventArgs eventArgs )
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
    [JSInvokable]
    public virtual Task OnDragOverHandler( DragEventArgs eventArgs )
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

    /// <summary>
    /// Event handler for <see cref="ContextMenu"/> event callback.
    /// </summary>
    /// <param name="eventArgs">Supplies information about an contextmenu event that is being raised.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual Task OnContextMenuHandler( MouseEventArgs eventArgs )
    {
        return ContextMenu.InvokeAsync( EventArgsMapper.ToMouseEventArgs( eventArgs ) );
    }

    /// <summary>
    /// Configures the render tree to handle draggable events for the component.
    /// </summary>
    /// <remarks>This method sets up event handlers and optional default prevention for drag-and-drop events,
    /// including <c>dragstart</c>, <c>dragend</c>, <c>dragenter</c>, <c>dragleave</c>, and <c>drop</c>. Event handlers
    /// are only added if the corresponding <see cref="EventCallback"/> is assigned. Default browser behavior for these
    /// events can be suppressed by setting the appropriate <c>PreventDefault</c> property to <see
    /// langword="true"/>.</remarks>
    /// <param name="builder">The <see cref="RenderTreeBuilder"/> used to define the render tree.</param>
    protected void BuildDraggableEventsRenderTree( RenderTreeBuilder builder )
    {
        if ( DragStart.HasDelegate )
            builder.OnDragStart( this, EventCallback.Factory.Create<DragEventArgs>( this, OnDragStartHandler ) );

        if ( DragStartPreventDefault )
            builder.OnDragStartPreventDefault( true );

        if ( DragEnd.HasDelegate )
            builder.OnDragEnd( this, EventCallback.Factory.Create<DragEventArgs>( this, OnDragEndHandler ) );

        if ( DragEndPreventDefault )
            builder.OnDragEndPreventDefault( true );

        if ( DragEnter.HasDelegate )
            builder.OnDragEnter( this, EventCallback.Factory.Create<DragEventArgs>( this, OnDragEnterHandler ) );

        if ( DragEnterPreventDefault )
            builder.OnDragEnterPreventDefault( true );

        if ( DragLeave.HasDelegate )
            builder.OnDragLeave( this, EventCallback.Factory.Create<DragEventArgs>( this, OnDragLeaveHandler ) );

        if ( DragLeavePreventDefault )
            builder.OnDragLeavePreventDefault( true );

        if ( Drop.HasDelegate )
            builder.OnDrop( this, EventCallback.Factory.Create<DragEventArgs>( this, OnDropHandler ) );

        if ( DropPreventDefault )
            builder.OnDropPreventDefault( true );

        if ( ContextMenu.HasDelegate )
            builder.OnContextMenu( this, EventCallback.Factory.Create<MouseEventArgs>( this, OnContextMenuHandler ) );

        if ( ContextMenuPreventDefault )
            builder.OnContextMenuPreventDefault( true );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the valid draggable attribute value if <see cref="Draggable"/> is enabled.
    /// </summary>
    protected string DraggableString => Draggable ? "true" : null;

    /// <summary>
    /// Gets or sets the <see cref="JSDragDropModule"/> instance.
    /// </summary>
    [Inject] public IJSDragDropModule JSDragDropModule { get; set; }

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
    /// The DragEnd event is fired when a drag operation is being ended (by releasing a mouse button or hitting the escape key).
    /// </summary>
    [Parameter] public EventCallback<DragEventArgs> DragEnd { get; set; }

    /// <summary>
    /// Used to prevent the default action for an <see cref="DragEnd"/> event.
    /// </summary>
    [Parameter] public bool DragEndPreventDefault { get; set; }

    /// <summary>
    /// The DragEnter event is fired when a dragged element or text selection enters a valid drop target.
    /// </summary>
    [Parameter] public EventCallback<DragEventArgs> DragEnter { get; set; }

    /// <summary>
    /// Used to prevent the default action for an <see cref="DragEnter"/> event.
    /// </summary>
    [Parameter] public bool DragEnterPreventDefault { get; set; }

    /// <summary>
    /// The DragLeave event is fired when a dragged element or text selection leaves a valid drop target.
    /// </summary>
    [Parameter] public EventCallback<DragEventArgs> DragLeave { get; set; }

    /// <summary>
    /// Used to prevent the default action for an <see cref="DragLeave"/> event.
    /// </summary>
    [Parameter] public bool DragLeavePreventDefault { get; set; }

    /// <summary>
    /// The DragOver event is fired when an element or text selection is being dragged over a valid drop target (every few hundred milliseconds).
    /// </summary>
    [Parameter] public EventCallback<DragEventArgs> DragOver { get; set; }

    /// <summary>
    /// Used to prevent the default action for an <see cref="DragOver"/> event.
    /// </summary>
    [Parameter] public bool DragOverPreventDefault { get; set; }

    /// <summary>
    /// The DragStart event is fired when the user starts dragging an element or text selection.
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

    /// <summary>
    /// The event is fired when an element or text selection is right clicked to show the context menu.
    /// </summary>
    [Parameter] public EventCallback<BLMouseEventArgs> ContextMenu { get; set; }

    /// <summary>
    /// Used to prevent the default action for an <see cref="ContextMenu"/> event.
    /// </summary>
    [Parameter] public bool ContextMenuPreventDefault { get; set; }

    #endregion
}