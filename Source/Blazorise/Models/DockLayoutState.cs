using System.Collections.Generic;

namespace Blazorise;

/// <summary>
/// Represents the mutable docking state for a <see cref="DockLayout"/>.
/// </summary>
/// <remarks>
/// The state can be persisted or edited directly. Call <see cref="DockLayout.Refresh"/> after changing an assigned instance in place.
/// Its tree, pane, rail, and restore-placement models are part of this advanced editing API.
/// Pane names are the stable identities used when the state is restored.
/// </remarks>
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