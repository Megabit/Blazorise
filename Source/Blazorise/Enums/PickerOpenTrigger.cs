#region Using directives
using System;
#endregion

namespace Blazorise;

/// <summary>
/// Defines which interactions can open a date or time picker menu.
/// </summary>
[Flags]
public enum PickerOpenTrigger
{
    /// <summary>
    /// No user interaction opens the picker menu automatically.
    /// </summary>
    None = 0,

    /// <summary>
    /// Clicking or tapping the picker input opens its menu.
    /// </summary>
    Click = 1 << 0,

    /// <summary>
    /// Focusing the picker input through keyboard navigation or code opens its menu.
    /// </summary>
    Focus = 1 << 1,

    /// <summary>
    /// Pressing a supported opening key opens the picker menu.
    /// </summary>
    OpenKeys = 1 << 2,

    /// <summary>
    /// All supported interactions open the picker menu.
    /// </summary>
    All = Click | Focus | OpenKeys,
}