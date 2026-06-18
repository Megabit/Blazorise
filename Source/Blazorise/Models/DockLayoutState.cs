using System.Collections.Generic;

namespace Blazorise;

/// <summary>
/// Represents the mutable docking state for a <see cref="DockLayout"/>.
/// </summary>
public class DockLayoutState
{
    /// <summary>
    /// Defines the root dock node.
    /// </summary>
    public DockNodeState Root { get; set; }

    /// <summary>
    /// Defines the persisted panel states.
    /// </summary>
    public List<DockPanelState> Panels { get; set; } = new();

    /// <summary>
    /// Defines the active panel name for each dock position.
    /// </summary>
    public Dictionary<DockPanelPosition, string> ActivePanels { get; set; } = new();
}