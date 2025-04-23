namespace Blazorise.DataGrid;

/// <summary>
/// Specifies the number of rows to export from a data grid.
/// </summary>
public class DataGridExportOptions
{
    /// <summary>
    /// -1 means all rows
    /// </summary>
    public int NumberOfRows { get; init; } = -1;
}