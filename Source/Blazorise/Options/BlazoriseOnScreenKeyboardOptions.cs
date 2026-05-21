#region Using directives
using System;
#endregion

namespace Blazorise;

/// <summary>
/// Represents the options available for configuring the on-screen keyboard.
/// </summary>
public class BlazoriseOnScreenKeyboardOptions
{
    /// <summary>
    /// If true, input components can show the on-screen keyboard when they receive focus.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// If true, the on-screen keyboard is shown when an enabled input receives focus.
    /// </summary>
    public bool ShowOnFocus { get; set; } = true;

    /// <summary>
    /// If true, the on-screen keyboard is hidden when the active input loses focus.
    /// </summary>
    public bool HideOnBlur { get; set; } = true;

    /// <summary>
    /// If true, the on-screen keyboard is hidden when the enter key is pressed.
    /// </summary>
    public bool HideOnEnter { get; set; }

    /// <summary>
    /// Gets or sets the default keyboard placement.
    /// </summary>
    public OnScreenKeyboardPlacement Placement { get; set; } = OnScreenKeyboardPlacement.Bottom;

    /// <summary>
    /// Gets or sets the default keyboard layout. When not set, each input component chooses its own layout.
    /// </summary>
    public OnScreenKeyboardLayout? DefaultLayout { get; set; }

    /// <summary>
    /// Gets or sets a predicate that decides if the keyboard should be shown for a focused input.
    /// </summary>
    public Func<OnScreenKeyboardContext, bool> ShouldShow { get; set; }

    /// <summary>
    /// Gets or sets a function that resolves the keyboard layout for a focused input.
    /// </summary>
    public Func<OnScreenKeyboardContext, OnScreenKeyboardLayout> LayoutSelector { get; set; }
}