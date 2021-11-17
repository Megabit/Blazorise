#region Using directives
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

        /// <summary>
        /// Default constructor for <see cref="BreakpointActivatorAdapter"/>.
        /// </summary>
        /// <param name="component">Reference to the breakpoint activator.</param>
        public BreakpointActivatorAdapter( IBreakpointActivator component )
        {
            this.component = component;
        }

        /// <summary>
        /// Gets the breakpoint element id.
        /// </summary>
        /// <returns>A breakpoint element id.</returns>
        [JSInvokable()]
        public string GetElementId()
        {
            return component.ElementId;
        }

        /// <summary>
        /// Notify us from JS that breakpoint has activated.
        /// </summary>
        /// <param name="breakpoint">Breakpoint name.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [JSInvokable()]
        public Task OnBreakpoint( string breakpoint )
        {
            return component.OnBreakpoint( IsBroken( component.Breakpoint, breakpoint ) );
        }

        /// <summary>
        /// Indicates if breakpoint was broken.
        /// </summary>
        /// <param name="checkBreakpoint">A breakpoint to check.</param>
        /// <param name="currentBreakpointString">Current breakpoint name.</param>
        /// <returns>True if breakpoint was broken.</returns>
        public static bool IsBroken( Breakpoint checkBreakpoint, string currentBreakpointString )
        {
            return checkBreakpoint > GetBreakpoint( currentBreakpointString );
        }

        /// <summary>
        /// Gets the <see cref="Breakpoint"/> by its name.
        /// </summary>
        /// <param name="breakpoint">Breakpoint name.</param>
        /// <returns>Returns <see cref="Breakpoint"/>.</returns>
        private static Breakpoint GetBreakpoint( string breakpoint )
        {
            return breakpoint switch
            {
                "mobile" => Breakpoint.Mobile,
                "tablet" => Breakpoint.Tablet,
                "desktop" => Breakpoint.Desktop,
                "widescreen" => Breakpoint.Widescreen,
                "fullhd" => Breakpoint.FullHD,
                _ => Breakpoint.None,
            };
        }
    }
}
