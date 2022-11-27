#region Using directives
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
}