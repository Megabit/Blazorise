#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Contracts for the tooltip JS module.
/// </summary>
public interface IJSTooltipModule : IBaseJSModule,
    IJSDestroyableModule
{
    /// <summary>
    /// Initializes the new tooltip within the JS module.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <param name="options">Additional options for the tooltip initialization.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Initialize( ElementReference elementRef, string elementId, TooltipInitializeJSOptions options );

    /// <summary>
    /// Updated tooltip content.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <param name="content">New content value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UpdateContent( ElementReference elementRef, string elementId, string content );
}