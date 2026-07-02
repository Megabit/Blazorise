#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders report items that can be dragged into the report designer surface.
/// </summary>
public partial class _ReportDesignerToolboxPanel
{
    #region Properties

    /// <summary>
    /// Raised when a toolbox item starts dragging.
    /// </summary>
    [Parameter] public EventCallback<ReportTreeNodeDragEventArgs> ToolboxNodeDragStarted { get; set; }

    /// <summary>
    /// Raised when a tree node drag operation ends.
    /// </summary>
    [Parameter] public EventCallback<ReportTreeNode> NodeDragEnded { get; set; }

    #endregion
}