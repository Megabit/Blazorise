using System.Collections.Generic;

namespace Blazorise;

/// <summary>
/// Represents the mutable docking state for a <see cref="DockLayout"/>.
/// </summary>
public class DockLayoutState
{
    /// <summary>
    /// Gets the current persistence schema version.
    /// </summary>
    public const int CurrentSchemaVersion = 1;

    /// <summary>
    /// Defines the persistence schema version.
    /// </summary>
    public int SchemaVersion { get; set; } = CurrentSchemaVersion;

    /// <summary>
    /// Defines the root dock node.
    /// </summary>
    public DockNodeState Root { get; set; }

    /// <summary>
    /// Defines the persisted pane states.
    /// </summary>
    public List<DockPaneState> Panes { get; set; } = new();

    /// <summary>
    /// Defines the auto-hide rails and their pane tabs.
    /// </summary>
    public List<DockRailState> Rails { get; set; } = new();
}