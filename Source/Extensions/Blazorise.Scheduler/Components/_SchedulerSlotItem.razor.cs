#region Using directives
using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Scheduler.Components;

/// <summary>
/// Represents a slot item within a scheduler, providing functionality for rendering, interaction, and event handling.
/// </summary>
/// <typeparam name="TItem">The type of the data item associated with the scheduler slot.</typeparam>
public partial class _SchedulerSlotItem<TItem>
{
    #region Members

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
    /// Handles the drag start event for the current item.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    protected Task OnDragStart( DragEventArgs eventArgs )
    {
        if ( DragStarted is not null )
            return DragStarted.Invoke( eventArgs, ViewItem );

        return Task.CompletedTask;
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

    #endregion

    #region Properties

    /// <summary>
    /// Gets a string that represents whether the slot is draggable.
    /// </summary>
    private string DraggableAttribute => Scheduler?.Editable == true && Scheduler?.UseInternalEditing == true && Scheduler?.Draggable == true ? "true" : "false";

    /// <summary>
    /// Provides access to the parent <see cref="Scheduler{TItem}"/> component.
    /// </summary>
    [CascadingParameter] public Scheduler<TItem> Scheduler { get; set; }

    /// <summary>
    /// Gets or sets the index of the currently rendered item.
    /// </summary>
    [Parameter] public int ItemIndex { get; set; }

    /// <summary>
    /// Gets or sets the total number of items to be rendered in a slot.
    /// </summary>
    [Parameter] public int TotalItems { get; set; }

    /// <summary>
    /// Gets or sets the view information for a scheduler item.
    /// </summary>
    [Parameter] public SchedulerItemViewInfo<TItem> ViewItem { get; set; }

    /// <summary>
    /// Gets or sets the start time of the slot.
    /// </summary>
    [Parameter] public DateTime SlotStart { get; set; }

    /// <summary>
    /// Gets or sets the end time of the slot.
    /// </summary>
    [Parameter] public DateTime SlotEnd { get; set; }

    /// <summary>
    /// Gets or sets the cell height used to render items.
    /// </summary>
    [Parameter] public double ItemCellHeight { get; set; }

    /// <summary>
    /// Callback triggered when an item is edited.
    /// </summary>
    [Parameter] public Func<SchedulerItemViewInfo<TItem>, DateTime, DateTime, Task> EditItemClicked { get; set; }

    /// <summary>
    /// Callback triggered when an item is deleted.
    /// </summary>
    [Parameter] public Func<SchedulerItemViewInfo<TItem>, Task> DeleteItemClicked { get; set; }

    /// <summary>
    /// Callback triggered when an item is dragged.
    /// </summary>
    [Parameter] public Func<DragEventArgs, SchedulerItemViewInfo<TItem>, Task> DragStarted { get; set; }

    #endregion
}
