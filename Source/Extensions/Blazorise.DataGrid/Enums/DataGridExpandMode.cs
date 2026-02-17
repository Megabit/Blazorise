namespace Blazorise.DataGrid;

/// <summary>
/// Defines the expand behavior for self-referencing rows in the data grid.
/// </summary>
public enum DataGridExpandMode
{
    /// <summary>
    /// Allows only one self-reference row to stay expanded at a time.
    /// </summary>
    Single,

    /// <summary>
    /// Allows multiple self-reference rows to stay expanded.
    /// </summary>
    Multiple,
}