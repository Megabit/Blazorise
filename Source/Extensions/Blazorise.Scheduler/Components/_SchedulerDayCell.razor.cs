#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Scheduler.Components;

/// <summary>
/// Represents a single time slot cell in the day view of the <see cref="Scheduler{TItem}"/> component.
/// </summary>
/// <typeparam name="TItem">The type of the scheduler item.</typeparam>
public partial class _SchedulerDayCell<TItem>
{
    #region Methods

    /// <summary>
    /// Handles the click event on a time slot.
    /// </summary>
    /// <param name="start">The start time of the slot.</param>
    /// <param name="end">The end time of the slot.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    protected Task OnSlotClicked( DateTime start, DateTime end )
    {
        return Scheduler.NotifySlotClicked( start, end );
    }

    /// <summary>
    /// Handles the click event on an item to edit it.
    /// </summary>
    /// <param name="viewItem">The view item to edit.</param>
    /// <param name="start">The start time of the slot.</param>
    /// <param name="end">The end time of the slot.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    protected Task OnEditItemClicked( SchedulerItemViewInfo<TItem> viewItem, DateTime start, DateTime end )
    {
        if ( viewItem is not null )
        {
            return Scheduler.NotifyEditItemClicked( viewItem.Item );
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the click event to delete an item.
    /// </summary>
    /// <param name="viewInfo">The view info of the item to delete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    protected Task OnDeleteItemClicked( SchedulerItemViewInfo<TItem> viewInfo )
    {
        if ( viewInfo is not null )
        {
            return Scheduler.NotifyDeleteItemClicked( viewInfo.Item );
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets the start time of the specified slot index.
    /// </summary>
    /// <param name="slotIndex">The index of the slot.</param>
    /// <returns>The start time of the slot.</returns>
    protected DateTime GetSlotStart( int slotIndex )
    {
        if ( SlotsPerCell <= 0 )
        {
            return new DateTime( Date.Year, Date.Month, Date.Day, Time.Hour, 0, 0 );
        }

        var slotDuration = TimeSpan.FromHours( 1.0 / SlotsPerCell );
        var startTime = slotDuration * ( slotIndex - 1 );

        return new DateTime( Date.Year, Date.Month, Date.Day, Time.Hour, startTime.Minutes, 0 );
    }

    /// <summary>
    /// Gets the end time of the specified slot index.
    /// </summary>
    /// <param name="slotIndex">The index of the slot.</param>
    /// <returns>The end time of the slot.</returns>
    protected DateTime GetSlotEnd( int slotIndex )
    {
        if ( SlotsPerCell <= 0 )
        {
            return new DateTime( Date.Year, Date.Month, Date.Day, Time.Hour + 1, 0, 0 );
        }

        var slotDuration = TimeSpan.FromHours( 1.0 / SlotsPerCell );
        var endTime = slotDuration * slotIndex;

        return new DateTime( Date.Year, Date.Month, Date.Day, Time.Hour, 0, 0 ).Add( endTime );
    }

    /// <summary>
    /// Gets the view items that intersect with the specified time range.
    /// </summary>
    /// <param name="start">The start time of the range.</param>
    /// <param name="end">The end time of the range.</param>
    /// <returns>A collection of <see cref="SchedulerItemViewInfo{TItem}"/> instances.</returns>
    protected IEnumerable<SchedulerItemViewInfo<TItem>> GetSlotItemViewsInfo( DateTime start, DateTime end )
    {
        if ( Scheduler is null )
            return default;

        return Scheduler.GetViewItemInRange( ViewItems, start, end );
    }

    /// <summary>
    /// Gets the relative time from the start of the hour for a given slot index.
    /// </summary>
    /// <param name="slotIndex">The index of the slot.</param>
    /// <returns>The relative <see cref="TimeSpan"/> from the hour's start.</returns>
    protected TimeSpan GetTime( int slotIndex )
    {
        if ( SlotsPerCell <= 0 )
            return TimeSpan.Zero;

        var slotDuration = TimeSpan.FromHours( 1.0 / SlotsPerCell );
        var time = slotDuration * ( slotIndex - 1 );

        return time;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the background color for the slot based on the workday range.
    /// </summary>
    private Blazorise.Background BackgroundColor => WorkDayStart is not null && WorkDayEnd is not null && !( Time >= WorkDayStart.Value && Time < WorkDayEnd.Value )
        ? Blazorise.Background.Light
        : Blazorise.Background.Default;

    /// <summary>
    /// Gets or sets the parent scheduler component.
    /// </summary>
    [CascadingParameter] public Scheduler<TItem> Scheduler { get; set; }

    /// <summary>
    /// Gets or sets the date for the cell.
    /// </summary>
    [Parameter] public DateOnly Date { get; set; }

    /// <summary>
    /// Gets or sets the time for the cell.
    /// </summary>
    [Parameter] public TimeOnly Time { get; set; }

    /// <summary>
    /// Gets or sets the start of the workday.
    /// </summary>
    [Parameter] public TimeOnly? WorkDayStart { get; set; }

    /// <summary>
    /// Gets or sets the end of the workday.
    /// </summary>
    [Parameter] public TimeOnly? WorkDayEnd { get; set; }

    /// <summary>
    /// Gets or sets the number of slots per hour.
    /// </summary>
    [Parameter] public int SlotsPerCell { get; set; }

    /// <summary>
    /// Gets or sets the height of the header cell in pixels.
    /// </summary>
    [Parameter] public double HeaderCellHeight { get; set; }

    /// <summary>
    /// Gets or sets the height of the item cell in pixels.
    /// </summary>
    [Parameter] public double ItemCellHeight { get; set; }

    /// <summary>
    /// Gets or sets the collection of view items to display in the cell.
    /// </summary>
    [Parameter] public IEnumerable<SchedulerItemViewInfo<TItem>> ViewItems { get; set; }

    /// <summary>
    /// Gets or sets the drag area used for appointment dragging.
    /// </summary>
    [Parameter] public SchedulerSection DragSection { get; set; }

    #endregion
}
