#region Using directives
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise
{
    [EventHandler( "onmouseenter", typeof( MouseEventArgs ), true, true )]
    [EventHandler( "onmouseleave", typeof( MouseEventArgs ), true, true )]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class EventHandlers
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
    }
}
