namespace Blazorise.DataGrid;

/// <summary>
/// Represents configuration options for exporting DataGrid content to the clipboard in CSV format.
/// </summary>
public  class CsvClipboardExportOptions: ClipboardExportOptions, ICsvExportOptions
{
    public bool ExportHeader { get; init; } = true;
}