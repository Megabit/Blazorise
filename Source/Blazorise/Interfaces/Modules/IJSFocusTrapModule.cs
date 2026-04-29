using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Modules;

/// <summary>
/// Contracts for the focus trap JS module.
/// </summary>
public interface IJSFocusTrapModule : IBaseJSModule, IJSDestroyableModule
{
    /// <summary>
    /// Initializes the focus trap.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Initialize( ElementReference elementRef, string elementId );

    /// <summary>
    /// Focuses the first focusable element inside the focus trap.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Focus( ElementReference elementRef, string elementId );
}