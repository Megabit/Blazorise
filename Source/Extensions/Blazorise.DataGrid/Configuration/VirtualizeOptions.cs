namespace Blazorise.DataGrid;

/// <summary>
/// Defines the options for <see cref="DataGrid{TItem}"/> virtualize mode.
/// </summary>
public class VirtualizeOptions
{
    /// <summary>
    /// Sets the DataGrid height when <see cref="DataGrid.DataGrid{TItem}.Virtualize"/> feature is enabled (defaults to 500px).
    /// </summary>
    public string DataGridHeight { get; set; } = "500px";

    /// <summary>
    /// Sets the DataGrid height when <see cref="DataGrid.DataGrid{TItem}.Virtualize"/> feature is enabled (defaults to 500px).
    /// </summary>
    public string DataGridMaxHeight { get; set; } = "500px";

    /// <summary>
    /// Gets or sets a value that determines how many additional items will be rendered
    /// before and after the visible region. This help to reduce the frequency of rendering
    /// during scrolling. However, higher values mean that more elements will be present
    /// in the page.
    /// </summary>
    public int OverscanCount { get; set; } = 10;

    /// <summary>
    /// If DataGrid goes into DataGridEditMode.Inline or DataGridEditMode.Form, scrolls the row to the top.
    /// Defaults to true.
    /// </summary>
    public bool ScrollRowOnEdit { get; set; } = true;
}