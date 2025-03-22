#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Scheduler.Components;

public partial class _SchedulerDayCell<TItem>
{
    #region Members

    #endregion

    #region Methods

    protected async Task OnSlotClick( TimeSpan time )
    {
        Console.WriteLine( $"Slot clicked: {time}" );

        await Scheduler.NotifySlotClicked( Date, new TimeOnly( Hour, time.Minutes ), ( new TimeSpan( 1, 0, 0 ) / SlotsPerCell ) );
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

    [Parameter] public double CellHeight { get; set; }

    #endregion
}
