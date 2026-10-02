#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Visual splitter used by resizable dock panes.
/// </summary>
public partial class _DockSplitter : _BaseDockRenderer
{
    #region Methods

    /// <inheritdoc/>
    private protected override bool IsAffected( DockLayoutChange change )
        => change.Kind == DockLayoutChangeKind.Tree
            || change.Kind == DockLayoutChangeKind.Node
                && DockLayoutTreeQuery.FindNodeById( Context?.GetNode( NodeId ), change.NodeId ) is not null;

    private Task OnResizeEndedHandler( ResizerEventArgs eventArgs )
        => Context?.ResizeDockSplit( NodeId, eventArgs ) ?? Task.CompletedTask;

    #endregion

    #region Properties

    private double SplitterThickness => Context?.SplitterThickness ?? 6;

    private double SplitterOffset => SplitterThickness;

    /// <summary>
    /// Maps the splitter to the correct resize axis.
    /// </summary>
    private Orientation ResizeOrientation
        => SplitOrientation == Orientation.Horizontal
            ? Blazorise.Orientation.Vertical
            : Blazorise.Orientation.Horizontal;

    /// <summary>
    /// Places the resizer on the boundary shared with the adjacent node.
    /// </summary>
    private Placement ResizePlacement
        => ResizeOrientation == Blazorise.Orientation.Vertical
            ? Blazorise.Placement.End
            : Blazorise.Placement.Bottom;

    /// <summary>
    /// Anchors the absolute resizer to the first track only on the active split axis. The cross axis remains the full split container.
    /// </summary>
    private string GridPlacementStyle
        => ResizeOrientation == Blazorise.Orientation.Vertical
            ? "grid-column:1 / 2;"
            : "grid-row:1 / 2;";

    private ResizerTargets Targets => Context?.GetDockResizeTargets( NodeId );

    /// <summary>
    /// Defines whether the owning split arranges its tracks horizontally or vertically.
    /// </summary>
    [Parameter] public Orientation SplitOrientation { get; set; }

    /// <summary>
    /// Identifies the split node whose start and end tracks are resized.
    /// </summary>
    [Parameter] public string NodeId { get; set; }

    #endregion
}