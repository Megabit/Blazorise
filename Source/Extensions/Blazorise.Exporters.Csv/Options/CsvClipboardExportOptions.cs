namespace Blazorise.Exporters.Csv;

/// <summary>
/// Represents configuration options for exporting DataGrid content to the clipboard in CSV format.
/// </summary>
public class CsvClipboardExportOptions : ClipboardExportOptions, ICsvExportOptions
{
    /// <summary>
    /// Gets a value indicating whether to include the header row in the exported CSV data. Default is <c>true</c>.
    /// </summary>
    public bool ExportHeader { get; init; } = true;
}