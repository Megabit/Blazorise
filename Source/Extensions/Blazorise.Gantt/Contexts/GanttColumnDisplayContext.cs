#region Using directives
using System;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Gantt;

/// <summary>
/// Context passed to <see cref="BaseGanttColumn{TItem}.DisplayTemplate"/>.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public class GanttColumnDisplayContext<TItem>
{
    /// <summary>
    /// Creates a new <see cref="GanttColumnDisplayContext{TItem}"/>.
    /// </summary>
    public GanttColumnDisplayContext( Gantt<TItem> gantt, BaseGanttColumn<TItem> column, TItem item, string key, object value, string text, int level, bool hasChildren, bool expanded, bool selected, bool focused, Func<Task> toggleNode, double treeToggleWidth )
    {
        Gantt = gantt;
        Column = column;
        Item = item;
        Key = key;
        Value = value;
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
    /// Parent gantt.
    /// </summary>
    public Gantt<TItem> Gantt { get; }

    /// <summary>
    /// Column.
    /// </summary>
    public BaseGanttColumn<TItem> Column { get; }

    /// <summary>
    /// Row item.
    /// </summary>
    public TItem Item { get; }

    /// <summary>
    /// Row key.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Raw value.
    /// </summary>
    public object Value { get; }

    /// <summary>
    /// Formatted value text.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Tree level.
    /// </summary>
    public int Level { get; }

    /// <summary>
    /// True when row has children.
    /// </summary>
    public bool HasChildren { get; }

    /// <summary>
    /// True when row is expanded.
    /// </summary>
    public bool Expanded { get; }

    /// <summary>
    /// True when row is selected.
    /// </summary>
    public bool Selected { get; }

    /// <summary>
    /// True when row is focused.
    /// </summary>
    public bool Focused { get; }

    /// <summary>
    /// Callback that toggles current node.
    /// </summary>
    public Func<Task> ToggleNode { get; }

    /// <summary>
    /// Width reserved for tree toggle placeholder.
    /// </summary>
    public double TreeToggleWidth { get; }

}