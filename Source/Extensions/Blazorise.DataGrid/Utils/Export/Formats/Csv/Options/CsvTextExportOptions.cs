namespace Blazorise.DataGrid;

/// <summary>
/// Represents configuration options for exporting DataGrid content to a CSV-formatted text string.
/// </summary>
public class CsvTextExportOptions:TextExportOptions, ICsvExportOptions
{
    public bool ExportHeader { get; init; } = true;
}