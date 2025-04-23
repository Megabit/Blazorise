namespace Blazorise.Exporters.Csv;

/// <summary>
/// Represents configuration options for exporting DataGrid content to a CSV-formatted text string.
/// </summary>
public class CsvTextExportOptions : TextExportOptions, ICsvExportOptions
{
    /// <summary>
    /// Indicates whether to export the header. Defaults to true.
    /// </summary>
    public bool ExportHeader { get; init; } = true;
}