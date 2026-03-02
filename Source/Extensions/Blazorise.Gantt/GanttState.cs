#region Using directives
using System;
using System.Collections.Generic;
using Blazorise;
#endregion

namespace Blazorise.Gantt;

/// <summary>
/// Represents a persisted Gantt component state.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public class GanttState<TItem>
{
    /// <summary>
    /// Gets or sets current anchor date.
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Gets or sets selected view.
    /// </summary>
    public GanttView SelectedView { get; set; }

    /// <summary>
    /// Gets or sets current search text.
    /// </summary>
    public string SearchText { get; set; }

    /// <summary>
    /// Gets or sets current sort field.
    /// </summary>
    public string SortField { get; set; }

    /// <summary>
    /// Gets or sets current sort direction.
    /// </summary>
    public SortDirection SortDirection { get; set; } = SortDirection.Default;

    /// <summary>
    /// Gets or sets current selected row.
    /// </summary>
    public TItem SelectedRow { get; set; }

    /// <summary>
    /// Gets or sets currently focused row key.
    /// </summary>
    public string FocusedRowKey { get; set; }

    /// <summary>
    /// Gets or sets collapsed tree row keys.
    /// </summary>
    public List<string> CollapsedRowKeys { get; set; } = new();

    /// <summary>
    /// Gets or sets tree list width in pixels.
    /// </summary>
    public double? TreeListWidth { get; set; }

    /// <summary>
    /// Gets or sets current edit state.
    /// </summary>
    public GanttEditState EditState { get; set; } = GanttEditState.None;

    /// <summary>
    /// Gets or sets current edited item.
    /// </summary>
    public TItem EditItem { get; set; }

    /// <summary>
    /// Gets or sets current edited parent item.
    /// </summary>
    public TItem EditParentItem { get; set; }

    /// <summary>
    /// Gets or sets persisted tree column states.
    /// </summary>
    public List<GanttColumnState> ColumnStates { get; set; } = new();

}