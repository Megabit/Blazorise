#region Using directives
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Scheduler.Components;

public partial class _SchedulerDayCell<TItem>
{
    #region Members

    #endregion

    #region Methods

    protected async Task OnSlotClicked( TItem item, DateTime start, DateTime end )
    {
        if ( item is not null )
        {
            await Scheduler.NotifyItemClicked( item );

            return;
        }

        await Scheduler.NotifySlotClicked( start, end );
    }

    protected DateTime GetSlotStart( int slotIndex )
    {
        if ( SlotsPerCell <= 0 )
        {
            return new DateTime( Date.Year, Date.Month, Date.Day, Hour, 0, 0 );
        }

        var slotDuration = TimeSpan.FromHours( 1.0 / SlotsPerCell );
        var startTime = slotDuration * ( slotIndex - 1 );

        return new DateTime( Date.Year, Date.Month, Date.Day, Hour, startTime.Minutes, 0 );
    }

    protected DateTime GetSlotEnd( int slotIndex )
    {
        if ( SlotsPerCell <= 0 )
        {
            return new DateTime( Date.Year, Date.Month, Date.Day, Hour + 1, 0, 0 );
        }

        var slotDuration = TimeSpan.FromHours( 1.0 / SlotsPerCell );
        var endTime = slotDuration * slotIndex;

        return new DateTime( Date.Year, Date.Month, Date.Day, Hour, 0, 0 ).Add( endTime );
    }

    protected TItem GetSlotAppointment( DateTime start, DateTime end )
    {
        if ( Scheduler is null )
            return default;

        return Scheduler.ItemsInRange( start, end )
            .Where( x => !Scheduler.GetItemAllDay( x ) )
            .FirstOrDefault();
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

    private Blazorise.Background BackgroundColor => WorkDayStart is not null && WorkDayEnd is not null && !( Hour >= WorkDayStart.Value.Hour && Hour <= WorkDayEnd.Value.Hour )
        ? Blazorise.Background.Light
        : Blazorise.Background.Default;

    [CascadingParameter] public Scheduler<TItem> Scheduler { get; set; }

    [Parameter] public DateOnly Date { get; set; }

    [Parameter] public int Hour { get; set; }

    [Parameter] public TimeOnly? WorkDayStart { get; set; }

    [Parameter] public TimeOnly? WorkDayEnd { get; set; }

    [Parameter] public int SlotsPerCell { get; set; }

    [Parameter] public double HeaderCellHeight { get; set; }

    [Parameter] public double ItemCellHeight { get; set; }

    #endregion
}
