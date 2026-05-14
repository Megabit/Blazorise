#region Using directives
using System;
using System.Threading.Tasks;
#endregion

namespace Blazorise;

/// <summary>
/// Represents an active gesture binding on an element.
/// </summary>
public interface IGestureSubscription : IAsyncDisposable
{
    /// <summary>
    /// Updates the active gesture options and event handlers.
    /// </summary>
    /// <param name="options">Gesture options.</param>
    /// <param name="eventHandlers">Gesture event handlers.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Update( GestureOptions options, GestureEventHandlers eventHandlers );
}