#region Using directives
using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise
{
    [EventHandler( "onmouseenter", typeof( MouseEventArgs ), true, true )]
    [EventHandler( "onmouseleave", typeof( MouseEventArgs ), true, true )]
    public static class EventHandlers
    {
    }
}
