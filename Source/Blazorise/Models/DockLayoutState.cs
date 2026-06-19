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
    /// Defines the persisted pane states.
    /// </summary>
    public List<DockPaneState> Panes { get; set; } = new();

}