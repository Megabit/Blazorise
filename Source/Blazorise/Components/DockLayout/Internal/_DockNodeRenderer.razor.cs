#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Renders a node in a <see cref="DockLayout"/> tree.
/// </summary>
public partial class _DockNodeRenderer : ComponentBase
{
    #region Properties

    private string SplitClass => Node?.Orientation == DockSplitOrientation.Vertical
        ? "dock-split dock-split-vertical"
        : "dock-split dock-split-horizontal";

    private string SplitStyle => Layout?.GetDockSplitStyle( Node );

    private DockPanePosition FirstSplitterDock => Node?.Orientation == DockSplitOrientation.Vertical
        ? DockPanePosition.Top
        : DockPanePosition.Left;

    private DockPanePosition SecondSplitterDock => Node?.Orientation == DockSplitOrientation.Vertical
        ? DockPanePosition.Bottom
        : DockPanePosition.Right;

    private bool CanResize => Layout is not null && Node?.Kind == DockNodeKind.Split && SplitterDock is not null && !string.IsNullOrWhiteSpace( SplitNodeId );

    /// <summary>
    /// Gets or sets the owner dock layout.
    /// </summary>
    [Parameter] public DockLayout Layout { get; set; }

    /// <summary>
    /// Gets or sets the node to render.
    /// </summary>
    [Parameter] public DockNodeState Node { get; set; }

    /// <summary>
    /// Gets or sets the local splitter side for the rendered node.
    /// </summary>
    [Parameter] public DockPanePosition? SplitterDock { get; set; }

    /// <summary>
    /// Gets or sets the split node that owns the rendered node splitter.
    /// </summary>
    [Parameter] public string SplitNodeId { get; set; }

    #endregion
}