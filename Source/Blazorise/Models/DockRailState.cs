using System.Collections.Generic;

namespace Blazorise;

/// <summary>
/// Represents the auto-hide rail state for one side of a <see cref="DockLayout"/>.
/// </summary>
public class DockRailState
{
    /// <summary>
    /// Defines the side where auto-hidden pane tabs are shown.
    /// </summary>
    public DockPanePosition Position { get; set; }

    /// <summary>
    /// Defines the auto-hidden pane tabs shown in the rail.
    /// </summary>
    public List<DockRailItemState> Items { get; set; } = new();
}