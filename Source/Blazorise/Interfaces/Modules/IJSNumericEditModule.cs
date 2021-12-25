#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules
{
    /// <summary>
    /// Contracts for the <see cref="NumericEdit{TValue}"/> JS module.
    /// </summary>
    public interface IJSNumericEditModule : IBaseJSModule,
        IJSDestroyableModule
    {
        /// <summary>
        /// Initializes the new <see cref="TextEdit"/> within the JS module.
        /// </summary>
        /// <param name="dotNetObjectRef">Reference to the numeric adapter.</param>
        /// <param name="elementRef">Reference to the rendered element.</param>
        /// <param name="elementId">ID of the rendered element.</param>
        /// <param name="options">Options for numeric edit.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask Initialize( DotNetObjectReference<NumericEditAdapter> dotNetObjectRef, ElementReference elementRef, string elementId, object options );

        /// <summary>
        /// Updates the <see cref="NumericEdit{TValue}"/> options.
        /// </summary>
        /// <param name="elementRef">Reference to the rendered element.</param>
        /// <param name="elementId">ID of the rendered element.</param>
        /// <param name="options">Options for numeric edit.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask UpdateOptions( ElementReference elementRef, string elementId, object options );

        ValueTask UpdateValue<TValue>( ElementReference elementRef, string elementId, TValue value );
    }
}
