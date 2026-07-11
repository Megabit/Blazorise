#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Represents a node rendered by the internal report tree view.
/// </summary>
public sealed class ReportTreeNode
{
    /// <summary>
    /// Stable tree node key used for rendering and collapse state.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Main node label.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Secondary node text shown on the right side of the row.
    /// </summary>
    public string Detail { get; set; }

    /// <summary>
    /// Node kind used to choose icon and styling.
    /// </summary>
    public ReportTreeNodeKind Kind { get; set; }

    /// <summary>
    /// Indicates that clicking the node can select a report object.
    /// </summary>
    public bool Selectable { get; set; }

    /// <summary>
    /// Marks the node as the active selection.
    /// </summary>
    public bool Selected { get; set; }

    /// <summary>
    /// Enables drag start behavior for the node.
    /// </summary>
    public bool Draggable { get; set; }

    /// <summary>
    /// Custom value used by callbacks to resolve report objects.
    /// </summary>
    public object Value { get; set; }

    /// <summary>
    /// Child nodes rendered below this node when expanded.
    /// </summary>
    public List<ReportTreeNode> Children { get; set; } = [];
}