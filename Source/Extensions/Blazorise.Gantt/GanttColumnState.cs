#region Using directives
using Blazorise;
#endregion

namespace Blazorise.Gantt;

/// <summary>
/// Represents persisted state for a single Gantt tree column.
/// </summary>
public class GanttColumnState
{
    /// <summary>
    /// Gets or sets unique column key.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Gets or sets column field name.
    /// </summary>
    public string Field { get; set; }

    /// <summary>
    /// Gets or sets whether the column is visible.
    /// </summary>
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Gets or sets column display order.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets column width.
    /// </summary>
    public FluentUnitValue Width { get; set; }
}