using Microsoft.AspNetCore.Components.Web;

namespace Blazorise.Reporting.Internal;

public sealed class ReportTreeNodeMouseEventArgs( ReportTreeNode node, MouseEventArgs mouseEventArgs )
{
    public ReportTreeNode Node { get; } = node;

    public MouseEventArgs MouseEventArgs { get; } = mouseEventArgs;
}

public sealed class ReportTreeNodeDragEventArgs( ReportTreeNode node, DragEventArgs dragEventArgs )
{
    public ReportTreeNode Node { get; } = node;

    public DragEventArgs DragEventArgs { get; } = dragEventArgs;
}