#region Using directives
using System;
#endregion

namespace Blazorise;

/// <summary>
/// Defines the input types that can be enabled by the global on-screen keyboard option.
/// </summary>
[Flags]
public enum OnScreenKeyboardInputType
{
    /// <summary>
    /// No input type.
    /// </summary>
    None = 0,

    /// <summary>
    /// Text-like inputs.
    /// </summary>
    Text = 1,

    /// <summary>
    /// Numeric inputs.
    /// </summary>
    Numeric = 2,

    /// <summary>
    /// Date inputs.
    /// </summary>
    Date = 4,

    /// <summary>
    /// Time inputs.
    /// </summary>
    Time = 8,

    /// <summary>
    /// Picker components.
    /// </summary>
    Pickers = 16,

    /// <summary>
    /// All input types.
    /// </summary>
    All = Text | Numeric | Date | Time | Pickers,
}