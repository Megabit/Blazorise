using System;
using System.Threading.Tasks;

namespace Blazorise.Gantt;

/// <summary>
/// Context for rendering tree command cell content.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public class GanttTreeCommandCellContext<TItem>
{
    /// <summary>
    /// Creates a new context.
    /// </summary>
    /// <param name="gantt">Parent Gantt component.</param>
    /// <param name="item">The data item for current row.</param>
    /// <param name="key">Unique row key.</param>
    /// <param name="level">Tree depth level.</param>
    /// <param name="hasChildren">Indicates whether row has child items.</param>
    /// <param name="expanded">Indicates whether row is expanded.</param>
    /// <param name="selected">Indicates whether row is selected.</param>
    /// <param name="focused">Indicates whether row is focused.</param>
    /// <param name="canAddChild">Indicates whether add-child action is available.</param>
    /// <param name="addChild">Callback used to add a child task.</param>
    /// <param name="addChildText">Localized add-child label.</param>
    public GanttTreeCommandCellContext( Gantt<TItem> gantt, TItem item, string key, int level, bool hasChildren, bool expanded, bool selected, bool focused, bool canAddChild, Func<Task> addChild, string addChildText )
    {
        Gantt = gantt;
        Item = item;
        Key = key;
        Level = level;
        HasChildren = hasChildren;
        Expanded = expanded;
        Selected = selected;
        Focused = focused;
        CanAddChild = canAddChild;
        AddChild = addChild;
        AddChildText = addChildText;
    }

    /// <summary>
    /// Gets the parent Gantt component.
    /// </summary>
    public Gantt<TItem> Gantt { get; }

    /// <summary>
    /// Gets the data item for current row.
    /// </summary>
    public TItem Item { get; }

    /// <summary>
    /// Gets unique row key.
    /// </summary>
    public string Key { get; }

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
    /// Gets whether add-child action is available.
    /// </summary>
    public bool CanAddChild { get; }

    /// <summary>
    /// Gets callback used to add a child task.
    /// </summary>
    public Func<Task> AddChild { get; }

    /// <summary>
    /// Gets localized add-child label.
    /// </summary>
    public string AddChildText { get; }
}