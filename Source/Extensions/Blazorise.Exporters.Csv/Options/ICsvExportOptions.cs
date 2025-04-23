namespace Blazorise.Exporters.Csv;

/// <summary>
/// Defines common CSV-specific export options shared across various export targets (e.g., file, clipboard, text).
/// </summary>
public interface ICsvExportOptions
{
    /// <summary>
    /// Indicates whether to export the header in a data export process.
    /// </summary>
    public bool ExportHeader { get; init; }
}