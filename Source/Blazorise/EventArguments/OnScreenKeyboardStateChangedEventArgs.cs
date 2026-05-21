#region Using directives
using System;
#endregion

namespace Blazorise;

/// <summary>
/// Supplies information about an on-screen keyboard state change.
/// </summary>
public class OnScreenKeyboardStateChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OnScreenKeyboardStateChangedEventArgs"/> class.
    /// </summary>
    /// <param name="state">Current on-screen keyboard state.</param>
    public OnScreenKeyboardStateChangedEventArgs( OnScreenKeyboardState state )
    {
        State = state;
    }

    /// <summary>
    /// Gets the current on-screen keyboard state.
    /// </summary>
    public OnScreenKeyboardState State { get; }
}