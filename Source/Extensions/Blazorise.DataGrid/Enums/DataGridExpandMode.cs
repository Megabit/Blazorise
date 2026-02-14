namespace Blazorise.DataGrid;

/// <summary>
/// Defines the expand behavior for hierarchical rows in the data grid.
/// </summary>
public enum DataGridExpandMode
{
    /// <summary>
    /// Allows only one hierarchy row to stay expanded at a time.
    /// </summary>
    Single,

    /// <summary>
    /// Allows multiple hierarchy rows to stay expanded.
    /// </summary>
    Multiple,
}