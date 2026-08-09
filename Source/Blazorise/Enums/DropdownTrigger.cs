using System;

namespace Blazorise;

/// <summary>
/// Defines which pointer interactions can trigger a <see cref="Dropdown"/> to open or close its menu.
/// </summary>
[Flags]
public enum DropdownTrigger
{
    /// <summary>
    /// No pointer interaction can open or close the menu.
    /// </summary>
    None = 0,

    /// <summary>
    /// Clicking the toggle can open or close the menu.
    /// </summary>
    Click = 1,

    /// <summary>
    /// Hovering the dropdown can open or close the menu.
    /// </summary>
    Hover = 2,

    /// <summary>
    /// Any supported pointer interaction can open or close the menu.
    /// </summary>
    All = Click | Hover,
}