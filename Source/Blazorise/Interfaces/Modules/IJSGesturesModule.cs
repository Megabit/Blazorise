#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Contracts for the <see cref="Gestures"/> JS module.
/// </summary>
public interface IJSGesturesModule : IBaseJSModule,
    IJSDestroyableModule
{
    /// <summary>
    /// Initializes the new <see cref="Gestures"/> within the JS module.
    /// </summary>
    /// <param name="dotNetObjectRef">Reference to the gestures adapter.</param>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <param name="options">Gestures options.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Initialize( DotNetObjectReference<GestureAdapter> dotNetObjectRef, ElementReference elementRef, string elementId, GesturesJSOptions options );

    /// <summary>
    /// Updates the <see cref="Gestures"/> options.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <param name="options">Gestures options.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UpdateOptions( ElementReference elementRef, string elementId, GesturesJSOptions options );
}