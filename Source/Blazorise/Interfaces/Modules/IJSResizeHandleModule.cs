#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Contracts for the <see cref="ResizeHandle"/> JavaScript module.
/// </summary>
public interface IJSResizeHandleModule : IBaseJSModule,
    IJSDestroyableModule
{
    /// <summary>
    /// Initializes a resize handle.
    /// </summary>
    /// <param name="dotNetObjectRef">Reference to the resize handle.</param>
    /// <param name="elementRef">Reference to the rendered handle element.</param>
    /// <param name="elementId">ID of the rendered handle element.</param>
    /// <param name="options">Resize handle options.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Initialize( DotNetObjectReference<ResizeHandle> dotNetObjectRef, ElementReference elementRef, string elementId, ResizeHandleJSOptions options );

    /// <summary>
    /// Updates resize handle options.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered handle element.</param>
    /// <param name="elementId">ID of the rendered handle element.</param>
    /// <param name="options">Resize handle options.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UpdateOptions( ElementReference elementRef, string elementId, ResizeHandleJSOptions options );
}