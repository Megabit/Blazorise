using Microsoft.AspNetCore.Components.Web;

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Event arguments for mouse actions raised by report tree nodes.
/// </summary>
/// <param name="node">Tree node associated with the mouse action.</param>
/// <param name="mouseEventArgs">Original Blazor mouse event arguments.</param>
public sealed class ReportTreeNodeMouseEventArgs( ReportTreeNode node, MouseEventArgs mouseEventArgs )
{
    /// <summary>
    /// Tree node associated with the mouse action.
    /// </summary>
    public ReportTreeNode Node { get; } = node;

    /// <summary>
    /// Original Blazor mouse event arguments.
    /// </summary>
    public MouseEventArgs MouseEventArgs { get; } = mouseEventArgs;
}

/// <summary>
/// Event arguments for drag actions raised by report tree nodes.
/// </summary>
/// <param name="node">Tree node associated with the drag action.</param>
/// <param name="dragEventArgs">Original Blazor drag event arguments.</param>
public sealed class ReportTreeNodeDragEventArgs( ReportTreeNode node, DragEventArgs dragEventArgs )
{
    /// <summary>
    /// Tree node associated with the drag action.
    /// </summary>
    public ReportTreeNode Node { get; } = node;

    /// <summary>
    /// Original Blazor drag event arguments.
    /// </summary>
    public DragEventArgs DragEventArgs { get; } = dragEventArgs;
}