#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Contracts for the <see cref="Resizer"/> JavaScript module.
/// </summary>
public interface IJSResizerModule : IBaseJSModule,
    IJSDestroyableModule
{
    /// <summary>
    /// Initializes a resizer.
    /// </summary>
    /// <param name="dotNetObjectRef">Reference to the resizer.</param>
    /// <param name="elementRef">Reference to the rendered resizer element.</param>
    /// <param name="elementId">ID of the rendered resizer element.</param>
    /// <param name="options">Resizer options.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Initialize( DotNetObjectReference<Resizer> dotNetObjectRef, ElementReference elementRef, string elementId, ResizerJSOptions options );

    /// <summary>
    /// Updates resizer options.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered resizer element.</param>
    /// <param name="elementId">ID of the rendered resizer element.</param>
    /// <param name="options">Resizer options.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UpdateOptions( ElementReference elementRef, string elementId, ResizerJSOptions options );
}