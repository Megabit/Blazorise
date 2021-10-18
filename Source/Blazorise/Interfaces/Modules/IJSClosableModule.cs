using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazorise.Modules
{
    /// <summary>
    /// Contracts for the closable JS module.
    /// </summary>
    public interface IJSClosableModule : IBaseJSModule
    {
        /// <summary>
        /// Registers the component to be listened for closing events.
        /// </summary>
        /// <param name="dotNetObjectRef">Reference to the activator adapter.</param>
        /// <param name="elementRef">Reference to the rendered element.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask Register( DotNetObjectReference<CloseActivatorAdapter> dotNetObjectRef, ElementReference elementRef );

        /// <summary>
        /// Removes the component from the closable listener.
        /// </summary>
        /// <param name="component">Component activator.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask Unregister( ICloseActivator component );
    }
}
