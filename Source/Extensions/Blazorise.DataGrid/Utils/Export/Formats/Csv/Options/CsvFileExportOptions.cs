namespace Blazorise.DataGrid;

/// <summary>
/// Represents the options for exporting data to a CSV file.
/// </summary>
public class CsvFileExportOptions: FileExportOptions,ICsvExportOptions
{
    public override  string FileExtension { get; init; } = "csv";
    public override string MimeType { get; init; } = "text/csv;charset=utf-8";

    public bool ExportHeader { get; init; } = true;
}

