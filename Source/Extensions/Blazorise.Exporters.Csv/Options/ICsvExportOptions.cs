namespace Blazorise.Exporters.Csv;

/// <summary>
/// Defines common CSV-specific export options shared across various export targets (e.g., file, clipboard, text).
/// </summary>
public interface ICsvExportOptions
{
    public bool ExportHeader { get; init; } 

}