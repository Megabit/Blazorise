#region Using directives
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders recursive tree view node containers for the internal report tree view.
/// </summary>
public partial class _ReportTreeViewNodes
{
    #region Methods

    private bool HasChildren( ReportTreeNode node )
    {
        return HasNodeChildren?.Invoke( node ) == true;
    }

    private bool IsExpanded( ReportTreeNode node )
    {
        return IsNodeExpanded?.Invoke( node ) == true;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Nodes rendered at the current recursion level.
    /// </summary>
    [Parameter] public IReadOnlyList<ReportTreeNode> Nodes { get; set; }

    /// <summary>
    /// Tree indentation level rendered by this component instance.
    /// </summary>
    [Parameter] public int Level { get; set; }

    /// <summary>
    /// Raised when a selectable tree node is clicked.
    /// </summary>
    [Parameter] public EventCallback<ReportTreeNode> NodeClicked { get; set; }

    /// <summary>
    /// Raised when a selectable tree node opens its context menu.
    /// </summary>
    [Parameter] public EventCallback<ReportTreeNodeMouseEventArgs> NodeContextMenu { get; set; }

    /// <summary>
    /// Raised when dragging starts for a draggable tree node.
    /// </summary>
    [Parameter] public EventCallback<ReportTreeNodeDragEventArgs> NodeDragStarted { get; set; }

    /// <summary>
    /// Raised when dragging ends for a draggable tree node.
    /// </summary>
    [Parameter] public EventCallback<ReportTreeNode> NodeDragEnded { get; set; }

    /// <summary>
    /// Determines whether the specified node is expanded.
    /// </summary>
    [Parameter] public Func<ReportTreeNode, bool> IsNodeExpanded { get; set; }

    /// <summary>
    /// Determines whether the specified node has children.
    /// </summary>
    [Parameter] public Func<ReportTreeNode, bool> HasNodeChildren { get; set; }

    /// <summary>
    /// Toggles the specified node expansion state.
    /// </summary>
    [Parameter] public Action<ReportTreeNode> ToggleNode { get; set; }

    #endregion
}