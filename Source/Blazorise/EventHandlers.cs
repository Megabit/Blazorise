#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise
{
    public delegate void ValidationStartedEventHandler();

    public delegate void ValidatingAllEventHandler( ValidatingAllEventArgs e );

    public delegate void ClearAllValidationsEventHandler();

    public delegate void ValidationsStatusChangedEventHandler( ValidationsStatusChangedEventArgs e );

    [EventHandler( "onmouseenter", typeof( MouseEventArgs ), true, true )]
    [EventHandler( "onmouseleave", typeof( MouseEventArgs ), true, true )]
    public static class EventHandlers
    {
    }
}
