#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Opens a menu of related ribbon commands from an icon and label.
/// </summary>
public partial class RibbonDropdown : BaseRibbonItem
{
    /// <summary>
    /// Controls the direction in which the menu opens. Defaults to <see cref="Direction.Down"/>.
    /// </summary>
    [Parameter] public Direction Direction { get; set; } = Direction.Down;

    /// <summary>
    /// Defines custom content for the menu trigger, replacing its default icon and label.
    /// </summary>
    [Parameter] public RenderFragment HeaderContent { get; set; }
}