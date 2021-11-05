#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules
{
    /// <summary>
    /// Contracts for the <see cref="InputMask"/> JS module.
    /// </summary>
    public interface IJSInputMaskModule : IBaseJSModule,
        IJSDestroyableModule
    {
        /// <summary>
        /// Initializes the new <see cref="InputMask"/> within the JS module.
        /// </summary>
        /// <param name="dotNetObjectRef">Reference to the color picker.</param>
        /// <param name="elementRef">Reference to the rendered element.</param>
        /// <param name="elementId">ID of the rendered element.</param>
        /// <param name="options">Additional options for the tooltip initialization.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask Initialize( DotNetObjectReference<InputMask> dotNetObjectRef, ElementReference elementRef, string elementId, object options );
    }
}
