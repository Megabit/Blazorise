#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;


/// <summary>
/// Contracts for the <see cref="Offcanvas"/> JS module.
/// </summary>
public interface IJSOffcanvasModule : IBaseJSModule
{
    /// <summary>
    /// Opens the Offcanvas component.
    /// </summary>
    /// <param name="elementRef">Reference to the Offcanvas component.</param>
    /// <param name="placement">The position of the Offcanvas.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask OpenOffcanvas( ElementReference elementRef, Placement placement);

    /// <summary>
    /// Closes the Offcanvas component.
    /// </summary>
    /// <param name="elementRef">Reference to the Offcanvas component.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask CloseOffcanvas( ElementReference elementRef );
}