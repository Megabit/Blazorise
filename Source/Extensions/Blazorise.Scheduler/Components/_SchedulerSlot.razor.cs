#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using static Blazorise.Scheduler.Utilities.FluentConstants;
#endregion

namespace Blazorise.Scheduler.Components;

/// <summary>
/// Represents a slot in a scheduler that can handle mouse events and manage items within it.
/// </summary>
/// <typeparam name="TItem">Represents the type of items that can be scheduled and displayed in the slot.</typeparam>
public partial class _SchedulerSlot<TItem>
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

    /// <summary>
    /// Defines a static readonly instance of SchedulerItemStyling with a default background set to Info.
    /// </summary>
    private static readonly SchedulerItemStyling DefaultItemStyling = new SchedulerItemStyling
    {
        Background = Background.Info,
    };

    #endregion

    #region Methods

    /// <summary>
    /// Handles the mouse enter event and sets the hover flag.
    /// </summary>
    /// <param name="eventArgs">The mouse event arguments.</param>
    protected Task OnMouseEnter( MouseEventArgs eventArgs )
    {
        mouseHovering = true;

        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the mouse leave event and clears the hover flag.
    /// </summary>
    /// <param name="eventArgs">The mouse event arguments.</param>
    protected Task OnMouseLeave( MouseEventArgs eventArgs )
    {
        mouseHovering = false;

        return Task.CompletedTask;
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
    /// <param name="e">The drag event arguments.</param>
    /// <param name="viewItem">The item being dragged.</param>
    protected Task OnItemDragStart( DragEventArgs e, SchedulerItemViewInfo<TItem> viewItem )
    {
        mouseHovering = false;
        draggingOver = false;

        return Scheduler.StartDrag( viewItem.Item, DragSection );
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

        return Scheduler.DropSlotItem( SlotStart, SlotEnd, DragSection );
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

        return "border-bottom-style: dashed !important";
    }

    /// <summary>
    /// Returns a string representing the CSS class for a scheduler item. It includes a default class and appends a
    /// custom class if provided.
    /// </summary>
    /// <param name="customClass">Specifies an additional CSS class to be included in the returned string.</param>
    /// <returns>A string that combines a default class with the custom class if it is not empty.</returns>
    private string GetItemClass( string customClass )
    {
        if ( string.IsNullOrEmpty( customClass ) )
            return "b-scheduler-item";


        return $"b-scheduler-item {customClass}";
    }

    /// <summary>
    /// Computes the CSS style for a scheduler item within the slot.
    /// </summary>
    /// <param name="viewItem">The item to style.</param>
    /// <param name="index">The index of the item among all items in the slot.</param>
    /// <param name="totalItems">The total number of items in the slot.</param>
    /// <param name="customStyles">Any additional custom styles to apply.</param>
    private string GetItemStyle( SchedulerItemViewInfo<TItem> viewItem, int index, int totalItems, string customStyles )
    {
        var viewStart = viewItem.ViewStart;
        var viewEnd = viewItem.ViewEnd;
        var viewDuration = viewEnd - viewStart;
        var top = ( viewStart - SlotStart ).TotalMinutes / 60 * ItemCellHeight;
        var height = viewDuration.TotalMinutes / 60 * ItemCellHeight;

        if ( totalItems == 1 )
        {
            return $"cursor: pointer; left: 0; right: {5.ToString( CultureInfo.InvariantCulture )}%; top: {top.ToString( CultureInfo.InvariantCulture )}px; height: {height.ToString( CultureInfo.InvariantCulture )}px; z-index: 1;{customStyles}";
        }

        double slotRightGap = 5.0;
        double itemWidth = ( 100.0 - slotRightGap ) / totalItems;
        double leftPercentage = index * itemWidth;

        if ( index == totalItems - 1 )
        {
            double rightPercentage = slotRightGap;
            return $"cursor: pointer; left: {leftPercentage.ToString( CultureInfo.InvariantCulture )}%; right: {rightPercentage.ToString( CultureInfo.InvariantCulture )}%; top: {top.ToString( CultureInfo.InvariantCulture )}px; height: {height.ToString( CultureInfo.InvariantCulture )}px; z-index: 1;{customStyles}";
        }
        else
        {
            double rightPercentage = 100 - ( ( index + 1 ) * itemWidth );
            rightPercentage += 1;
            return $"cursor: pointer; left: {leftPercentage.ToString( CultureInfo.InvariantCulture )}%; right: {rightPercentage.ToString( CultureInfo.InvariantCulture )}%; top: {top.ToString( CultureInfo.InvariantCulture )}px; height: {height.ToString( CultureInfo.InvariantCulture )}px; z-index: 1;{customStyles}";
        }
    }

    #endregion

    #region Properties

    private bool IsDraggingOver => draggingOver && DragSection == Scheduler.CurrentDragSection;

    private bool IsEditAllowed => Scheduler?.Editable == true && Scheduler?.UseInternalEditing == true;

    /// <summary>
    /// Gets the bottom border style if this is not the last slot.
    /// </summary>
    private IFluentBorder SlotBorderColor => IsDraggingOver ? BorderIs1Dark : ( LastSlot ? null : BorderIs1OnBottom );

    /// <summary>
    /// Gets the background color of the slot based on mouse hover state.
    /// </summary>
    private Blazorise.Background SlotBackgroundColor => mouseHovering ? Background.Light : Background.Default;

    private SchedulerItemStyling GetItemStyling( TItem item )
    {
        if ( Scheduler?.ItemStyling is null )
            return DefaultItemStyling;

        var itemStyling = new SchedulerItemStyling
        {
            Background = Background.Info
        };

        Scheduler.ItemStyling( item, itemStyling );

        return itemStyling;
    }

    /// <summary>
    /// Gets a string that represents whether the slot is draggable.
    /// </summary>
    private string DraggableAttribute => Scheduler?.Editable == true && Scheduler?.UseInternalEditing == true && Scheduler?.Draggable == true ? "true" : "false";

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
    /// Gets or sets the drag area associated with the slot.
    /// </summary>
    [Parameter] public SchedulerSection DragSection { get; set; }

    #endregion
}
