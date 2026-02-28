using System;
using System.Threading.Tasks;

namespace Blazorise.Gantt;

/// <summary>
/// Context for rendering a tree data cell.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public class GanttTreeCellContext<TItem>
{
    /// <summary>
    /// Creates a new context.
    /// </summary>
    /// <param name="gantt">Parent Gantt component.</param>
    /// <param name="item">The data item.</param>
    /// <param name="key">Unique row key.</param>
    /// <param name="column">Column kind.</param>
    /// <param name="text">Default text representation for this cell.</param>
    /// <param name="level">Tree depth level.</param>
    /// <param name="hasChildren">Indicates whether row has child items.</param>
    /// <param name="expanded">Indicates whether row is expanded.</param>
    /// <param name="selected">Indicates whether row is selected.</param>
    /// <param name="focused">Indicates whether row is focused.</param>
    /// <param name="toggleNode">Callback used to toggle row expansion.</param>
    /// <param name="treeToggleWidth">Width used for toggle placeholder alignment.</param>
    public GanttTreeCellContext( Gantt<TItem> gantt, TItem item, string key, GanttTreeColumn column, string text, int level, bool hasChildren, bool expanded, bool selected, bool focused, Func<Task> toggleNode, double treeToggleWidth )
    {
        Gantt = gantt;
        Item = item;
        Key = key;
        Column = column;
        Text = text;
        Level = level;
        HasChildren = hasChildren;
        Expanded = expanded;
        Selected = selected;
        Focused = focused;
        ToggleNode = toggleNode;
        TreeToggleWidth = treeToggleWidth;
    }

    /// <summary>
    /// Gets the parent Gantt component.
    /// </summary>
    public Gantt<TItem> Gantt { get; }

    /// <summary>
    /// Gets the data item.
    /// </summary>
    public TItem Item { get; }

    /// <summary>
    /// Gets unique row key.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets tree column kind.
    /// </summary>
    public GanttTreeColumn Column { get; }

    /// <summary>
    /// Gets default text representation for this cell.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets tree depth level.
    /// </summary>
    public int Level { get; }

    /// <summary>
    /// Gets whether row has child items.
    /// </summary>
    public bool HasChildren { get; }

    /// <summary>
    /// Gets whether row is expanded.
    /// </summary>
    public bool Expanded { get; }

    /// <summary>
    /// Gets whether row is selected.
    /// </summary>
    public bool Selected { get; }

    /// <summary>
    /// Gets whether row is focused.
    /// </summary>
    public bool Focused { get; }

    /// <summary>
    /// Gets callback used to toggle row expansion.
    /// </summary>
    public Func<Task> ToggleNode { get; }

    /// <summary>
    /// Gets width used for toggle placeholder alignment.
    /// </summary>
    public double TreeToggleWidth { get; }

    /// <summary>
    /// Gets whether row can be toggled.
    /// </summary>
    public bool CanToggle => HasChildren && ToggleNode is not null;
}