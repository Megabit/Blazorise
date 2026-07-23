#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Contracts for the context menu JS module.
/// </summary>
public interface IJSContextMenuModule : IBaseJSModule,
    IJSDestroyableModule
{
    /// <summary>
    /// Initializes the new context menu within the JS module.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <param name="menuElementId">Context menu body element id.</param>
    /// <param name="clientX">The viewport X coordinate used as the context menu anchor.</param>
    /// <param name="clientY">The viewport Y coordinate used as the context menu anchor.</param>
    /// <param name="contextElementSelector">A selector for the element that owns the context menu point.</param>
    /// <param name="options">Additional options for the context menu initialization.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Initialize( ElementReference elementRef, string elementId, string menuElementId, double clientX, double clientY, string contextElementSelector, ContextMenuJSOptions options );
}