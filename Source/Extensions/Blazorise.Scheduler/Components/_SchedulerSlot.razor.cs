#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using static Blazorise.Scheduler.Utilities.FluentConstants;
#endregion

namespace Blazorise.Scheduler.Components;

/// <summary>
/// Represents a slot in a scheduler that can handle mouse events and manage items within it.
/// </summary>
/// <typeparam name="TItem">Represents the type of items that can be scheduled and displayed in the slot.</typeparam>
public class _SchedulerSlot<TItem> : ComponentBase
{
    #region Members

    /// <summary>
    /// Tracks whether the mouse is currently hovering over the slot.
    /// </summary>
    private bool mouseHovering;

    /// <summary>
    /// Indicates whether an object is currently being dragged over a specific area.
    /// </summary>
    private bool draggingOver;

    private bool isThrottled = false;

    private readonly TimeSpan throttleInterval = TimeSpan.FromMilliseconds( 150 );

    #endregion

    #region Methods

    /// <summary>
    /// Handles the mouse enter event and sets the hover flag.
    /// </summary>
    /// <param name="eventArgs">The mouse event arguments.</param>
    protected Task OnSlotMouseEnter( MouseEventArgs eventArgs )
    {
        mouseHovering = true;

        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the mouse leave event and clears the hover flag.
    /// </summary>
    /// <param name="eventArgs">The mouse event arguments.</param>
    protected Task OnSlotMouseLeave( MouseEventArgs eventArgs )
    {
        mouseHovering = false;

        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the mouse down event of the slot.
    /// </summary>
    /// <param name="eventArgs">The mouse event arguments.</param>
    protected Task OnSlotMouseDown( MouseEventArgs eventArgs )
    {
        return Scheduler.NotifySlotMouseDown( eventArgs, Section, SlotStart, SlotEnd );
    }

    /// <summary>
    /// Handles the mouse move event of the slot.
    /// </summary>
    /// <param name="eventArgs">The mouse event arguments.</param>
    protected async Task OnSlotMouseMove( MouseEventArgs eventArgs )
    {
        if ( isThrottled )
            return;

        isThrottled = true;

        try
        {
            await Scheduler.NotifySlotMouseMove( eventArgs, Section, SlotStart, SlotEnd );
        }
        finally
        {
            _ = Task.Run( async () =>
            {
                await Task.Delay( throttleInterval );
                isThrottled = false;
            } );
        }
    }

    /// <summary>
    /// Handles the mouse up event of the slot.
    /// </summary>
    /// <param name="eventArgs">The mouse event arguments.</param>
    protected Task OnSlotMouseUp( MouseEventArgs eventArgs )
    {
        return Scheduler.NotifySlotMouseUp( eventArgs, Section, SlotStart, SlotEnd );
    }

    /// <summary>
    /// Handles a click on the slot and triggers the <see cref="SlotClicked"/> event callback.
    /// </summary>
    protected Task OnSlotClicked()
    {
        if ( SlotClicked is null )
            return Task.CompletedTask;

        return SlotClicked.Invoke( SlotStart, SlotEnd );
    }

    /// <summary>
    /// Handles the drag start event for a scheduler item.
    /// </summary>
    /// <param name="eventArgs">The drag event arguments.</param>
    /// <param name="viewItem">The item being dragged.</param>
    protected Task OnItemDragStart( DragEventArgs eventArgs, SchedulerItemViewInfo<TItem> viewItem )
    {
        mouseHovering = false;
        draggingOver = false;

        return Scheduler.StartDrag( viewItem.Item, Section );
    }

    /// <summary>
    /// Handles the drag enter event on the slot.
    /// </summary>
    /// <param name="e">The drag event arguments.</param>
    protected Task OnSlotDragEnter( DragEventArgs e )
    {
        draggingOver = true;

        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the drag leave event on the slot.
    /// </summary>
    /// <param name="e">The drag event arguments.</param>
    protected Task OnSlotDragLeave( DragEventArgs e )
    {
        draggingOver = false;

        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the drop event on the slot.
    /// </summary>
    /// <param name="e">The drag event arguments.</param>
    protected Task OnSlotDrop( DragEventArgs e )
    {
        draggingOver = false;

        return Scheduler.DropSlotItem( SlotStart, SlotEnd, Section );
    }

    /// <summary>
    /// Handles the click event for editing an item in the slot.
    /// </summary>
    /// <param name="viewItem">The item to be edited.</param>
    protected Task OnEditItemClicked( SchedulerItemViewInfo<TItem> viewItem )
    {
        if ( EditItemClicked is null )
            return Task.CompletedTask;

        return EditItemClicked.Invoke( viewItem, SlotStart, SlotEnd );
    }

    /// <summary>
    /// Handles the click event for deleting an item in the slot.
    /// </summary>
    /// <param name="viewItem">The item to be deleted.</param>
    protected Task OnDeleteItemClicked( SchedulerItemViewInfo<TItem> viewItem )
    {
        if ( DeleteItemClicked is null )
            return Task.CompletedTask;

        return DeleteItemClicked.Invoke( viewItem );
    }

    /// <summary>
    /// Gets the custom border style for the slot if it is not the last slot.
    /// </summary>
    private string GetSlotStyle()
    {
        if ( LastSlot || IsDraggingOver )
            return null;

        return "border-bottom-style: dashed !important;";
    }

    /// <summary>
    /// Builds the render tree for the scheduler slot component.
    /// </summary>
    /// <remarks>This method is responsible for rendering the scheduler slot, including its attributes, event
    /// handlers,  and child content. It dynamically generates the slot's structure and styling based on the provided 
    /// data and configuration. The slot supports various interactions such as mouse events, drag-and-drop,  and click
    /// events.</remarks>
    /// <param name="builder">The <see cref="RenderTreeBuilder"/> used to construct the render tree for the component.</param>
    protected override void BuildRenderTree( RenderTreeBuilder builder )
    {
        builder.OpenComponent<Div>();

        builder.Attribute( nameof( Div.Class ), "b-scheduler-slot" );
        builder.Attribute( nameof( Div.Style ), GetSlotStyle() );
        builder.Attribute( nameof( Div.Position ), PositionRelative );
        builder.Attribute( nameof( Div.Margin ), MarginIsAuto );
        builder.Attribute( nameof( Div.Width ), WidthIs100 );
        builder.Attribute( nameof( Div.Height ), HeightIs100 );
        builder.Attribute( nameof( Div.Border ), SlotBorderColor );
        builder.Attribute( nameof( Div.Background ), SlotBackgroundColor );

        builder.Data( "slot-start", DataSlotStart );
        builder.Data( "slot-end", DataSlotEnd );

        builder.OnMouseEnter( this, EventCallback.Factory.Create<MouseEventArgs>( this, OnSlotMouseEnter ) );
        builder.OnMouseLeave( this, EventCallback.Factory.Create<MouseEventArgs>( this, OnSlotMouseLeave ) );

        builder.OnClick( this, EventCallback.Factory.Create<MouseEventArgs>( this, OnSlotClicked ) );

        if ( Scheduler.Draggable )
        {
            builder.Attribute( "ondragover", "event.preventDefault();" );
            builder.OnDragEnter( this, EventCallback.Factory.Create<DragEventArgs>( this, OnSlotDragEnter ) );
            builder.OnDragLeave( this, EventCallback.Factory.Create<DragEventArgs>( this, OnSlotDragLeave ) );
            builder.OnDrop( this, EventCallback.Factory.Create<DragEventArgs>( this, OnSlotDrop ) );
        }

        if ( Scheduler.SlotSelectionMode == SchedulerSlotSelectionMode.Mouse )
        {
            if ( Scheduler.IsSelecting )
            {
                builder.OnMouseMove( this, EventCallback.Factory.Create<MouseEventArgs>( this, OnSlotMouseMove ) );
            }
            else
            {
                builder.OnMouseDown( this, EventCallback.Factory.Create<MouseEventArgs>( this, OnSlotMouseDown ) );
            }

            builder.OnMouseUp( this, EventCallback.Factory.Create<MouseEventArgs>( this, OnSlotMouseUp ) );
        }

        builder.Attribute( "ChildContent", (RenderFragment)delegate ( RenderTreeBuilder childBuilder )
        {
            if ( ViewItems is not null )
            {
                var totalItems = ViewItems.Count;

                foreach ( var viewItem in ViewItems )
                {
                    var index = ViewItems.IndexOf( viewItem );

                    childBuilder.OpenComponent<_SchedulerSlotItem<TItem>>();

                    childBuilder.Attribute( nameof( _SchedulerSlotItem<TItem>.ViewItem ), viewItem );
                    childBuilder.Attribute( nameof( _SchedulerSlotItem<TItem>.ItemIndex ), index );
                    childBuilder.Attribute( nameof( _SchedulerSlotItem<TItem>.TotalItems ), totalItems );
                    childBuilder.Attribute( nameof( _SchedulerSlotItem<TItem>.SlotStart ), SlotStart );
                    childBuilder.Attribute( nameof( _SchedulerSlotItem<TItem>.SlotEnd ), SlotEnd );
                    childBuilder.Attribute( nameof( _SchedulerSlotItem<TItem>.ItemCellHeight ), ItemCellHeight );
                    childBuilder.Attribute( nameof( _SchedulerSlotItem<TItem>.EditItemClicked ), EditItemClicked );
                    childBuilder.Attribute( nameof( _SchedulerSlotItem<TItem>.DeleteItemClicked ), DeleteItemClicked );
                    childBuilder.Attribute( nameof( _SchedulerSlotItem<TItem>.DragStarted ), OnItemDragStart );

                    childBuilder.CloseComponent();
                }
            }
        } );

        builder.CloseComponent();
    }

    #endregion

    #region Properties

    private bool IsDraggingOver => draggingOver && Section == Scheduler.CurrentDragSection;

    /// <summary>
    /// Gets the bottom border style if this is not the last slot.
    /// </summary>
    private IFluentBorder SlotBorderColor => IsDraggingOver ? BorderIs1Dark : ( LastSlot ? null : BorderIs1OnBottom );

    /// <summary>
    /// Gets the background color of the slot based on mouse hover state.
    /// </summary>
    private Blazorise.Background SlotBackgroundColor => mouseHovering ? Background.Light : Background.Default;

    /// <summary>
    /// Gets a string that represents whether the slot is draggable.
    /// </summary>
    private string DraggableAttribute => Scheduler?.Editable == true && Scheduler?.UseInternalEditing == true && Scheduler?.Draggable == true ? "true" : "false";

    /// <summary>
    /// Gets a string that represents the slot start time.
    /// </summary>
    private string DataSlotStart => SlotStart.ToString( "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture );

    /// <summary>
    /// Gets a string that represents the slot end time.
    /// </summary>
    private string DataSlotEnd => SlotEnd.ToString( "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture );

    /// <summary>
    /// Provides access to the parent <see cref="Scheduler{TItem}"/> component.
    /// </summary>
    [CascadingParameter] public Scheduler<TItem> Scheduler { get; set; }

    /// <summary>
    /// Provides access to the current scheduler state.
    /// </summary>
    [CascadingParameter] public SchedulerState State { get; set; }

    /// <summary>
    /// Gets or sets the list of view items to display in the slot.
    /// </summary>
    [Parameter] public List<SchedulerItemViewInfo<TItem>> ViewItems { get; set; }

    /// <summary>
    /// Gets or sets the start time of the slot.
    /// </summary>
    [Parameter] public DateTime SlotStart { get; set; }

    /// <summary>
    /// Gets or sets the end time of the slot.
    /// </summary>
    [Parameter] public DateTime SlotEnd { get; set; }

    /// <summary>
    /// Indicates whether this slot is the last one in its container.
    /// </summary>
    [Parameter] public bool LastSlot { get; set; }

    /// <summary>
    /// Gets or sets the cell height used to render items.
    /// </summary>
    [Parameter] public double ItemCellHeight { get; set; }

    /// <summary>
    /// Callback triggered when the slot is clicked.
    /// </summary>
    [Parameter] public Func<DateTime, DateTime, Task> SlotClicked { get; set; }

    /// <summary>
    /// Callback triggered when an item is edited.
    /// </summary>
    [Parameter] public Func<SchedulerItemViewInfo<TItem>, DateTime, DateTime, Task> EditItemClicked { get; set; }

    /// <summary>
    /// Callback triggered when an item is deleted.
    /// </summary>
    [Parameter] public Func<SchedulerItemViewInfo<TItem>, Task> DeleteItemClicked { get; set; }

    /// <summary>
    /// Gets or sets the area to be used when initiating transactional operations.
    /// </summary>
    [Parameter] public SchedulerSection Section { get; set; }

    #endregion
}
