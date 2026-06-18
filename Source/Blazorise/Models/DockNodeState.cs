using System.Collections.Generic;

namespace Blazorise;

/// <summary>
/// Represents a node in a dock layout tree.
/// </summary>
public class DockNodeState
{
    /// <summary>
    /// Defines the node kind.
    /// </summary>
    public DockNodeKind Kind { get; set; }

    /// <summary>
    /// Defines the panel name when <see cref="Kind"/> is <see cref="DockNodeKind.Panel"/>.
    /// </summary>
    public string PanelName { get; set; }

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
    /// Defines the panel names when <see cref="Kind"/> is <see cref="DockNodeKind.Tabs"/>.
    /// </summary>
    public List<string> Panels { get; set; } = new();

    /// <summary>
    /// Defines the active panel when <see cref="Kind"/> is <see cref="DockNodeKind.Tabs"/>.
    /// </summary>
    public string ActivePanel { get; set; }

    /// <summary>
    /// Defines the size of the node when it represents a resizable dock group.
    /// </summary>
    public string Size { get; set; }
}