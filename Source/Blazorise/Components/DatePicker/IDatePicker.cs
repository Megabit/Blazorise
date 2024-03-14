#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Base interface for <see cref="DatePicker{TValue}"/> to be updated from javascript.
/// </summary>
/// <remarks>
/// This is needed to set the value from javascript because calling generic component directly is not supported by Blazor.
/// </remarks>
public interface IDatePicker
{
    /// <summary>
    /// Handler for @onkeydown event.
    /// </summary>
    /// <param name="eventArgs">Information about the keyboard down event.</param>
    /// <returns>Returns awaitable task</returns>
    Task OnKeyDownHandler( KeyboardEventArgs eventArgs );

    /// <summary>
    /// Handler for @onkeypress event.
    /// </summary>
    /// <param name="eventArgs">Information about the keyboard pressed event.</param>
    /// <returns>Returns awaitable task</returns>
    Task OnKeyPressHandler( KeyboardEventArgs eventArgs );

    /// <summary>
    /// Handler for @onkeyup event.
    /// </summary>
    /// <param name="eventArgs">Information about the keyboard up event.</param>
    /// <returns>Returns awaitable task</returns>
    Task OnKeyUpHandler( KeyboardEventArgs eventArgs );

    /// <summary>
    /// Handler for @onfocus event.
    /// </summary>
    /// <param name="eventArgs">Information about the focus event.</param>
    /// <returns>Returns awaitable task</returns>
    Task OnFocusHandler( FocusEventArgs eventArgs );

    /// <summary>
    /// Handler for @onfocusin event.
    /// </summary>
    /// <param name="eventArgs">Information about the focus event.</param>
    /// <returns>Returns awaitable task</returns>
    Task OnFocusInHandler( FocusEventArgs eventArgs );

    /// <summary>
    /// Handler for @onfocusout event.
    /// </summary>
    /// <param name="eventArgs">Information about the focus event.</param>
    /// <returns>Returns awaitable task</returns>
    Task OnFocusOutHandler( FocusEventArgs eventArgs );

    /// <summary>
    /// Handler for @onblur event.
    /// </summary>
    /// <param name="eventArgs">Information about the focus event.</param>
    /// <returns>Returns awaitable task</returns>
    Task OnBlurHandler( FocusEventArgs eventArgs );
}