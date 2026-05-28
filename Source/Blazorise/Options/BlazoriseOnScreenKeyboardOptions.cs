#region Using directives
using System;
using System.Collections.Generic;
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
    /// Gets or sets the input types enabled by the global on-screen keyboard option.
    /// </summary>
    public OnScreenKeyboardInputType InputTypes { get; set; } = OnScreenKeyboardInputType.Text | OnScreenKeyboardInputType.Numeric;

    /// <summary>
    /// If true, the focused input is scrolled into view when it would be covered by the on-screen keyboard.
    /// </summary>
    public bool AutoScroll { get; set; } = true;

    /// <summary>
    /// Gets or sets the viewport margin, in pixels, kept between the focused input and the on-screen keyboard during automatic scrolling.
    /// </summary>
    public int AutoScrollMargin { get; set; } = 12;

    /// <summary>
    /// Gets or sets how the on-screen keyboard is shown for enabled input components.
    /// </summary>
    public OnScreenKeyboardShowMode ShowMode { get; set; } = OnScreenKeyboardShowMode.Focus;

    /// <summary>
    /// If true, the on-screen keyboard is hidden when the active input loses focus.
    /// </summary>
    public bool HideOnBlur { get; set; } = true;

    /// <summary>
    /// If true, the on-screen keyboard is hidden when the enter key is pressed.
    /// </summary>
    public bool HideOnEnter { get; set; } = true;

    /// <summary>
    /// Gets or sets how the on-screen keyboard enter key should behave.
    /// </summary>
    public OnScreenKeyboardEnterKeyBehavior EnterKeyBehavior { get; set; } = OnScreenKeyboardEnterKeyBehavior.Default;

    /// <summary>
    /// Gets or sets the default keyboard placement.
    /// </summary>
    public OnScreenKeyboardPlacement Placement { get; set; } = OnScreenKeyboardPlacement.Bottom;

    /// <summary>
    /// Gets or sets the default keyboard visual width.
    /// </summary>
    public OnScreenKeyboardSize KeyboardSize { get; set; } = OnScreenKeyboardSize.Medium;

    /// <summary>
    /// Gets or sets the default key arrangement inside keyboard rows.
    /// </summary>
    public OnScreenKeyboardKeyLayout KeyLayout { get; set; } = OnScreenKeyboardKeyLayout.Centered;

    /// <summary>
    /// Gets or sets the base key width, in pixels, used by centered key layout.
    /// </summary>
    public int? KeyWidth { get; set; }

    /// <summary>
    /// Gets or sets the key minimum height, in pixels.
    /// </summary>
    public int? KeyMinHeight { get; set; }

    /// <summary>
    /// If true, the text keyboard includes a key that toggles special characters.
    /// </summary>
    public bool ShowSpecialCharactersKey { get; set; }

    /// <summary>
    /// Gets or sets the rows used when the special characters keyboard is active.
    /// </summary>
    public IReadOnlyList<IReadOnlyList<OnScreenKeyboardKey>> SpecialCharactersRows { get; set; }

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

    /// <summary>
    /// Gets or sets a function that resolves keyboard rows for a focused input.
    /// </summary>
    public Func<OnScreenKeyboardContext, IReadOnlyList<IReadOnlyList<OnScreenKeyboardKey>>> LayoutProvider { get; set; }
}