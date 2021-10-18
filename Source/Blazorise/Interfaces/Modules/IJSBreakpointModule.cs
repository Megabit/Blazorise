using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Blazorise.Modules
{
    /// <summary>
    /// Contracts for the closable JS module.
    /// </summary>
    public interface IJSBreakpointModule : IBaseJSModule
    {
        /// <summary>
        /// Registers the component to be listened for breakpoint events.
        /// </summary>
        /// <param name="dotNetObjectRef">Reference to the activator adapter.</param>
        /// <param name="elementId">ID of the rendered element.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask RegisterBreakpoint( DotNetObjectReference<BreakpointActivatorAdapter> dotNetObjectRef, string elementId );

        /// <summary>
        /// Removes the component from the breakpoint events.
        /// </summary>
        /// <param name="component">Component activator.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask UnregisterBreakpoint( IBreakpointActivator component );

        /// <summary>
        /// Gets the last breakpoint name.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask<string> GetBreakpoint();
    }
}
