namespace Blazorise.Exporters;

/// <summary>
/// Represents a exporter that handles object-based (typed or binary) cell values for formats like Excel or BSON.
/// </summary>
public interface IBinaryExporter<TExportResult, in TSourceData> : IExporter<TExportResult, TSourceData>
    where TExportResult : IExportResult
    where TSourceData : IExportableData<object>
{
}