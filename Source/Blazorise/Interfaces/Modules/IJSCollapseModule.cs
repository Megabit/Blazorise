#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Animates panel height using provider-supplied state classes.
/// </summary>
public interface IJSCollapseModule : IBaseJSModule, IJSDestroyableModule
{
    /// <summary>
    /// Observes visibility changes on a collapse panel.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered panel.</param>
    /// <param name="elementId">ID of the rendered panel.</param>
    /// <param name="options">Provider classes and the animation settings container.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Initialize( ElementReference elementRef, string elementId, CollapseJSOptions options );
}