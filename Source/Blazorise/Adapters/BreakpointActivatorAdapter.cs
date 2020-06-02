#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Middleman between the closable component and javascript.
    /// </summary>
    public class BreakpointActivatorAdapter
    {
        private readonly IBreakpointActivator component;

        public BreakpointActivatorAdapter( IBreakpointActivator component )
        {
            this.component = component;
        }

        [JSInvokable()]
        public string GetElementId()
        {
            return component.ElementId;
        }

        [JSInvokable()]
        public Task OnBreakpoint( string breakpoint )
        {
            return component.OnBreakpoint( IsBroken( component.Breakpoint, breakpoint ) );
        }

        public static bool IsBroken( Breakpoint checkBreakpoint, string currentBreakpointString )
        {
            return checkBreakpoint > GetBreakpoint( currentBreakpointString );
        }

        private static Breakpoint GetBreakpoint( string breakpoint )
        {
            switch ( breakpoint )
            {
                case "mobile":
                    return Breakpoint.Mobile;
                case "tablet":
                    return Breakpoint.Tablet;
                case "desktop":
                    return Breakpoint.Desktop;
                case "widescreen":
                    return Breakpoint.Widescreen;
                case "fullhd":
                    return Breakpoint.FullHD;
                default:
                    return Breakpoint.None;
            }
        }
    }
}
