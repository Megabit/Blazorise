#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Ribbon;

/// <summary>
/// Displays a ribbon menu using the existing dropdown behavior.
/// </summary>
public partial class RibbonDropdown : BaseRibbonItem
{
    /// <summary>
    /// Gets or sets the direction in which the menu opens.
    /// </summary>
    [Parameter] public Direction Direction { get; set; } = Direction.Down;

    /// <summary>
    /// Gets or sets content replacing the default dropdown icon and text.
    /// </summary>
    [Parameter] public RenderFragment HeaderContent { get; set; }
}