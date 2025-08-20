namespace Blazorise.DataGrid;

/// <summary>
/// Specifies the number of rows to export from a data grid.
/// </summary>
public class DataGridExportOptions
{
    /// <summary>
    /// Defines the number of rows to export from a data grid. If null, all rows will be exported.
    /// </summary>
    public int? NumberOfRows { get; init; }

    /// <summary>
    /// Defines the fields to export from a data grid. If null, all fields will be exported.
    /// </summary>
    public string[] Fields { get; init; } = null;

    /// <summary>
    /// Defines whether to include captions instead of field names in the export. Defaults to false.
    /// </summary>
    public bool UseCaptions { get; init; }
}