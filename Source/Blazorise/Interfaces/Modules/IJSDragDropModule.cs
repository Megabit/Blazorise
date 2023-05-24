#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Contracts for the tooltip JS module.
/// </summary>
public interface IJSDragDropModule : IBaseJSModule,
    IJSDestroyableModule
{
    /// <summary>
    /// Initializes the new dragdrop zone within the JS module.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Initialize( ElementReference elementRef, string elementId );

    /// <summary>
    /// Initializes throttled events for drag and drop events.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <param name="dotNetObjectReference">The DotNet ObjectReference so javascript can interop back to blazor.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask InitializeThrottledDragEvents<T>( ElementReference elementRef, string elementId, DotNetObjectReference<T> dotNetObjectReference ) where T : class;

    /// <summary>
    /// Destroys throttled events for drag and drop events.
    /// </summary>
    /// <param name="elementRef"></param>
    /// <param name="elementId"></param>
    /// <returns></returns>
    ValueTask DestroyThrottledDragEvents( ElementReference elementRef, string elementId );
}