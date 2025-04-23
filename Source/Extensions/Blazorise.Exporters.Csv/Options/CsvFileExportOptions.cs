namespace Blazorise.Exporters.Csv;

/// <summary>
/// Represents the options for exporting data to a CSV file.
/// </summary>
public class CsvFileExportOptions : FileExportOptions, ICsvExportOptions
{
    /// <summary>
    /// Represents the file extension for the object, initialized to 'csv'.
    /// </summary>
    public override string FileName { get; init; } = "exported-data.csv";

    /// <summary>
    /// Represents the MIME type for CSV files with UTF-8 character encoding. It is initialized to 'text/csv;charset=utf-8'.
    /// </summary>
    public override string MimeType { get; init; } = "text/csv;charset=utf-8";

    /// <summary>
    /// Indicates whether to export the header. Defaults to true.
    /// </summary>
    public bool ExportHeader { get; init; } = true;
}