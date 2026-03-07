using System;

namespace Blazorise;

/// <summary>
/// Defines which interactions can trigger the <see cref="BarDropdownToggle"/> to open or close its menu.
/// </summary>
[Flags]
public enum BarDropdownToggleTrigger
{
    /// <summary>
    /// Uses legacy behavior.
    /// If <see cref="BaseLinkComponent.To"/> is set, icon click toggles and toggle-area click navigates.
    /// Otherwise, toggle-area click toggles.
    /// </summary>
    Auto = 0,

    /// <summary>
    /// Clicking the toggle area can open or close the menu.
    /// </summary>
    ToggleClick = 1,

    /// <summary>
    /// Clicking the toggle icon can open or close the menu.
    /// </summary>
    IconClick = 2,

    /// <summary>
    /// Route activation (URL match) can open or close the menu.
    /// </summary>
    RouteMatch = 4,

    /// <summary>
    /// Either toggle-area click or icon click can open or close the menu.
    /// </summary>
    ClickAny = ToggleClick | IconClick,

    /// <summary>
    /// All supported triggers can open or close the menu.
    /// </summary>
    All = ClickAny | RouteMatch,
}