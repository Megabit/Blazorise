namespace Blazorise.Exporters;

/// <summary>
/// Represents an exporter that handles string-based (textual) cell values, such as CSV or plain text formats.
/// </summary>
public interface ITextExporter<TExportResult, in TSourceData> : IExporter<TExportResult, TSourceData>
    where TExportResult : IExportResult
    where TSourceData : IExportableData<string>
{
}