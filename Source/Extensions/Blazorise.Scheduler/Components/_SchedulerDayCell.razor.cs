#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Scheduler.Components;

public partial class _SchedulerDayCell<TItem>
{
    #region Members

    #endregion

    #region Methods

    protected Task OnSlotClicked( DateTime start, DateTime end )
    {
        return Scheduler.NotifySlotClicked( start, end );
    }

    protected Task OnEditItemClicked( SchedulerItemViewInfo<TItem> viewItem, DateTime start, DateTime end )
    {
        if ( viewItem is not null )
        {
            return Scheduler.NotifyEditItemClicked( viewItem );
        }

        return Task.CompletedTask;
    }

    protected Task OnDeleteItemClicked( SchedulerItemViewInfo<TItem> viewInfo )
    {
        if ( viewInfo is not null )
        {
            return Scheduler.NotifyDeleteItemClicked( viewInfo );
        }

        return Task.CompletedTask;
    }

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

    protected IEnumerable<SchedulerItemViewInfo<TItem>> GetSlotItemViewsInfo( DateTime start, DateTime end )
    {
        if ( Scheduler is null )
            return default;

        return Scheduler.GetViewItemInRange( ViewItems, start, end );
    }

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

    private Blazorise.Background BackgroundColor => WorkDayStart is not null && WorkDayEnd is not null && !( Time >= WorkDayStart.Value && Time < WorkDayEnd.Value )
        ? Blazorise.Background.Light
        : Blazorise.Background.Default;

    [CascadingParameter] public Scheduler<TItem> Scheduler { get; set; }

    [Parameter] public DateOnly Date { get; set; }

    [Parameter] public TimeOnly Time { get; set; }

    [Parameter] public TimeOnly? WorkDayStart { get; set; }

    [Parameter] public TimeOnly? WorkDayEnd { get; set; }

    [Parameter] public int SlotsPerCell { get; set; }

    [Parameter] public double HeaderCellHeight { get; set; }

    [Parameter] public double ItemCellHeight { get; set; }

    [Parameter] public IEnumerable<SchedulerItemViewInfo<TItem>> ViewItems { get; set; }

    #endregion
}
