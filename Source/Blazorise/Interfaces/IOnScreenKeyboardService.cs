#region Using directives
using System;
using System.Threading.Tasks;
#endregion

namespace Blazorise;

/// <summary>
/// Defines a service that controls the on-screen keyboard.
/// </summary>
public interface IOnScreenKeyboardService
{
    /// <summary>
    /// Occurs when the keyboard state changes.
    /// </summary>
    event EventHandler<OnScreenKeyboardStateChangedEventArgs> StateChanged;

    /// <summary>
    /// Gets the current keyboard state.
    /// </summary>
    OnScreenKeyboardState State { get; }

    /// <summary>
    /// Shows the keyboard for the supplied input context.
    /// </summary>
    /// <param name="context">Input context.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Show( OnScreenKeyboardContext context );

    /// <summary>
    /// Hides the keyboard.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Hide();

    /// <summary>
    /// Hides the keyboard if it currently belongs to the supplied element.
    /// </summary>
    /// <param name="elementId">Input element id.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Hide( string elementId );

    /// <summary>
    /// Handles a key press.
    /// </summary>
    /// <param name="key">Pressed key.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PressKey( OnScreenKeyboardKey key );

    /// <summary>
    /// Inserts text into the active input.
    /// </summary>
    /// <param name="text">Text to insert.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task InsertText( string text );

    /// <summary>
    /// Removes the last character from the active input.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Backspace();

    /// <summary>
    /// Clears the active input.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Clear();

    /// <summary>
    /// Confirms the active input.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Enter();
}