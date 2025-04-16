#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Scheduler.Components;

public partial class _SchedulerSlot<TItem>
{
    #region Members

    private bool mouseHovering;

    #endregion

    #region Methods

    protected Task OnMouseEnter( MouseEventArgs eventArgs )
    {
        mouseHovering = true;

        return Task.CompletedTask;
    }

    protected Task OnMouseLeave( MouseEventArgs eventArgs )
    {
        mouseHovering = false;

        return Task.CompletedTask;
    }

    protected Task OnSlotClicked()
    {
        if ( SlotClicked is null )
            return Task.CompletedTask;

        return SlotClicked.Invoke( SlotStart, SlotEnd );
    }

    protected Task OnItemDragStart( DragEventArgs e, SchedulerItemViewInfo<TItem> viewItem )
    {
        mouseHovering = false;

        return Scheduler.StartDrag( viewItem.Item );
    }

    protected Task OnSlotDrop( DragEventArgs e )
    {
        return Scheduler.DropSlotItem( SlotStart, SlotEnd );
    }

    protected Task OnEditItemClicked( SchedulerItemViewInfo<TItem> viewItem )
    {
        if ( EditItemClicked is null )
            return Task.CompletedTask;

        return EditItemClicked.Invoke( viewItem.Item, SlotStart, SlotEnd );
    }

    protected Task OnDeleteItemClicked( SchedulerItemViewInfo<TItem> viewItem )
    {
        if ( DeleteItemClicked is null )
            return Task.CompletedTask;

        return DeleteItemClicked.Invoke( viewItem );
    }

    private string GetSlotStyle()
    {
        if ( LastSlot )
            return null;

        return "border-bottom-style: dashed !important";
    }

    private string GetItemStyle( SchedulerItemViewInfo<TItem> viewItem, int index, int totalItems )
    {
        var viewStart = viewItem.ViewStart;
        var viewEnd = viewItem.ViewEnd;
        var viewDuration = viewEnd - viewStart;
        var top = ( viewStart - SlotStart ).TotalMinutes / 60 * ItemCellHeight;
        var height = viewDuration.TotalMinutes / 60 * ItemCellHeight;

        // For single items, leave a small gap on the right (5% of width)
        if ( totalItems == 1 )
        {
            return $"cursor: pointer; left: 0; right: {5.ToString( CultureInfo.InvariantCulture )}%; top: {top.ToString( CultureInfo.InvariantCulture )}px; height: {height.ToString( CultureInfo.InvariantCulture )}px; z-index: 1;";
        }

        // For multiple items, distribute them evenly but leave a small gap at the end
        double slotRightGap = 5.0; // 5% gap on the far right side of the slot
        double itemWidth = ( 100.0 - slotRightGap ) / totalItems;
        double leftPercentage = index * itemWidth;

        // Calculate right percentage
        if ( index == totalItems - 1 )
        {
            // Last item should extend to the start of the gap
            double rightPercentage = slotRightGap;
            return $"cursor: pointer; left: {leftPercentage.ToString( CultureInfo.InvariantCulture )}%; right: {rightPercentage.ToString( CultureInfo.InvariantCulture )}%; top: {top.ToString( CultureInfo.InvariantCulture )}px; height: {height.ToString( CultureInfo.InvariantCulture )}px; z-index: 1;";
        }
        else
        {
            // Middle items should maintain their position with the original 1% overlap
            double rightPercentage = 100 - ( ( index + 1 ) * itemWidth );
            rightPercentage += 1; // Add the 1% overlap
            return $"cursor: pointer; left: {leftPercentage.ToString( CultureInfo.InvariantCulture )}%; right: {rightPercentage.ToString( CultureInfo.InvariantCulture )}%; top: {top.ToString( CultureInfo.InvariantCulture )}px; height: {height.ToString( CultureInfo.InvariantCulture )}px; z-index: 1;";
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the border style of the slot.
    /// </summary>
    private IFluentBorder BottomBorder => LastSlot ? null : Border.Is1.OnBottom;

    /// <summary>
    /// Gets the background color of the slot.
    /// </summary>
    private Blazorise.Background BackgroundColor => mouseHovering ? Background.Light : Background.Default;

    /// <summary>
    /// Returns a string indicating whether the Scheduler is draggable. It returns 'true' if draggable, otherwise 'false'.
    /// </summary>
    private string DraggableAttribute => Scheduler?.Draggable == true ? "true" : "false";

    [CascadingParameter] public Scheduler<TItem> Scheduler { get; set; }

    [CascadingParameter] public SchedulerState State { get; set; }

    /// <summary>
    /// Defines the appointment that is displayed in the slot.
    /// </summary>
    [Parameter] public List<SchedulerItemViewInfo<TItem>> ViewItems { get; set; }

    /// <summary>
    /// Defines the start date of the slot.
    /// </summary>
    [Parameter] public DateTime SlotStart { get; set; }

    /// <summary>
    /// Defines the end date of the slot.
    /// </summary>
    [Parameter] public DateTime SlotEnd { get; set; }

    /// <summary>
    /// Defines if this is the last slot in the cell.
    /// </summary>
    [Parameter] public bool LastSlot { get; set; }

    /// <summary>
    /// Defines the cell height of the item.
    /// </summary>
    [Parameter] public double ItemCellHeight { get; set; }

    [Parameter] public Func<DateTime, DateTime, Task> SlotClicked { get; set; }

    [Parameter] public Func<TItem, DateTime, DateTime, Task> EditItemClicked { get; set; }

    [Parameter] public Func<SchedulerItemViewInfo<TItem>, Task> DeleteItemClicked { get; set; }

    #endregion
}
