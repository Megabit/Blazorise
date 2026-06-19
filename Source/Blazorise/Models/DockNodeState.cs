using System.Collections.Generic;

namespace Blazorise;

/// <summary>
/// Represents a node in a dock layout tree.
/// </summary>
public class DockNodeState
{
    /// <summary>
    /// Identifies the node inside the dock layout tree.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Defines the node kind.
    /// </summary>
    public DockNodeKind Kind { get; set; }

    /// <summary>
    /// Defines the pane name when <see cref="Kind"/> is <see cref="DockNodeKind.Pane"/>.
    /// </summary>
    public string PaneName { get; set; }

    /// <summary>
    /// Defines the first child node when <see cref="Kind"/> is <see cref="DockNodeKind.Split"/>.
    /// </summary>
    public DockNodeState First { get; set; }

    /// <summary>
    /// Defines the second child node when <see cref="Kind"/> is <see cref="DockNodeKind.Split"/>.
    /// </summary>
    public DockNodeState Second { get; set; }

    /// <summary>
    /// Defines the split orientation when <see cref="Kind"/> is <see cref="DockNodeKind.Split"/>.
    /// </summary>
    public DockSplitOrientation Orientation { get; set; }

    /// <summary>
    /// Defines the first child ratio when <see cref="Kind"/> is <see cref="DockNodeKind.Split"/>.
    /// </summary>
    public double Ratio { get; set; } = 0.25;

    /// <summary>
    /// Defines whether split child tracks are calculated from <see cref="Ratio"/> instead of child dock sizes.
    /// </summary>
    public bool UseRatio { get; set; }

    /// <summary>
    /// Defines the pane names when <see cref="Kind"/> is <see cref="DockNodeKind.Tabs"/>.
    /// </summary>
    public List<string> Panes { get; set; } = new();

    /// <summary>
    /// Defines the active pane when <see cref="Kind"/> is <see cref="DockNodeKind.Tabs"/>.
    /// </summary>
    public string ActivePane { get; set; }

    /// <summary>
    /// Defines the size of the node when it represents a resizable dock group.
    /// </summary>
    public string Size { get; set; }
}