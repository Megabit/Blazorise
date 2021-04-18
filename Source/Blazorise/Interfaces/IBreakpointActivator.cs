#region Using directives
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Interface to be used for the "togglers" that are responsible to open/close other components(dropdown, bar, modal, etc.).
    /// </summary>
    public interface IBreakpointActivator
    {
        /// <summary>
        /// Gets the id of the component that has activated the close procedure.
        /// </summary>
        string ElementId { get; }

        /// <summary>
        /// Gets the breakpoint.
        /// </summary>
        Breakpoint Breakpoint { get; }

        /// <summary>
        /// Triggers the component to activate breakpoint.
        /// </summary>
        /// <param name="broken">The reason for closing the component.</param>
        Task OnBreakpoint( bool broken );
    }
}
