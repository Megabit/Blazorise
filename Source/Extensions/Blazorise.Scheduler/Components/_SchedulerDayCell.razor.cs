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

        await Scheduler.NotifySlotClicked( Date, new TimeOnly( Hour, time.Minutes ) );
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

    [CascadingParameter] public Scheduler<TItem> Scheduler { get; set; }

    [Parameter] public DateOnly Date { get; set; }

    [Parameter] public int Hour { get; set; }

    [Parameter] public int SlotsPerCell { get; set; }

    #endregion
}
