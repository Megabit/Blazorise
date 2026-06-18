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

    /// <summary>
    /// Gets or sets the owner dock layout.
    /// </summary>
    [Parameter] public DockLayout Layout { get; set; }

    /// <summary>
    /// Gets or sets the node to render.
    /// </summary>
    [Parameter] public DockNodeState Node { get; set; }

    #endregion
}