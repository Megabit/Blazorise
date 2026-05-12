#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Provides gesture binding services for existing rendered elements.
/// </summary>
public interface IGestureService
{
    /// <summary>
    /// Attaches gesture handling to an element.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="options">Gesture options.</param>
    /// <param name="eventHandlers">Gesture event handlers.</param>
    /// <returns>An active gesture subscription.</returns>
    ValueTask<IGestureSubscription> Attach( ElementReference elementRef, GestureOptions options, GestureEventHandlers eventHandlers );

    /// <summary>
    /// Attaches gesture handling to an element.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">Optional ID of the rendered element.</param>
    /// <param name="options">Gesture options.</param>
    /// <param name="eventHandlers">Gesture event handlers.</param>
    /// <returns>An active gesture subscription.</returns>
    ValueTask<IGestureSubscription> Attach( ElementReference elementRef, string elementId, GestureOptions options, GestureEventHandlers eventHandlers );
}