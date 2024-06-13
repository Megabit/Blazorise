#region Using directives
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Extensions;

/// <summary>
/// EventArgs derived classes extensions.
/// </summary>
public static class EventArgsExtensions
{
    /// <summary>
    /// Returns true if a modifier key was pressed.
    /// </summary>
    public static bool IsModifierKey( this KeyboardEventArgs eventArgs )
        => eventArgs.AltKey || eventArgs.CtrlKey || eventArgs.ShiftKey || eventArgs.MetaKey;

    /// <summary>
    /// Returns true if a text key was pressed.
    /// </summary>
    public static bool IsTextKey( this KeyboardEventArgs eventArgs )
        => eventArgs.Key is not null && Regex.IsMatch( eventArgs.Key, @"^\b\w{1}$" );
}