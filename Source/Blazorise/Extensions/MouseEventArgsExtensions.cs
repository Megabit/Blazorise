#region Using directives
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Extensions;

/// <summary>
/// Provides a method to map <see cref="MouseEventArgs"/> button values to <see cref="MouseButton"/> values.
/// </summary>
/// <remarks>This utility class is designed to convert the button information from a <see cref="MouseEventArgs"/>
/// instance into a corresponding <see cref="MouseButton"/> enumeration value. The mapping is based on the button codes
/// provided by <see cref="MouseEventArgs"/>.</remarks>
public static class MouseEventArgsExtensions
{
    /// <summary>
    /// Converts a <see cref="MouseEventArgs"/> button value to a corresponding <see cref="MouseButton"/> enumeration.
    /// </summary>
    /// <param name="eventArgs">The mouse event arguments containing the button information to convert. Cannot be <see langword="null"/>.</param>
    /// <returns>A <see cref="MouseButton"/> value corresponding to the button in <paramref name="eventArgs"/>: <see
    /// cref="MouseButton.Middle"/> for button value 1, <see cref="MouseButton.Right"/> for button value 2,  and <see
    /// cref="MouseButton.Left"/> for all other values.</returns>
    public static MouseButton ToMouseButton( this MouseEventArgs eventArgs )
    {
        return eventArgs.Button switch
        {
            1 => MouseButton.Middle,
            2 => MouseButton.Right,
            _ => MouseButton.Left,
        };
    }
}