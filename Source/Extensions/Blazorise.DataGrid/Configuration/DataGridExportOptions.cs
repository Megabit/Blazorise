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

    /// <summary>
    /// null means All fields
    /// </summary>
    public string[] Fields { get; init; } = null;

    /// <summary>
    /// if true, the Captions will be used for export column names.
    /// if false  Field is used.
    /// </summary>
    public bool UseCaptions { get; init; } = true;
}