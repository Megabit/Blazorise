using System.Threading.Tasks;

namespace Blazorise.Exporters;

/// <summary>
/// Defines an interface for exporting data, returning a result based on the provided source data.
/// </summary>
/// <typeparam name="TExportResult">Represents the type of result produced by the export operation.</typeparam>
/// <typeparam name="TSourceData">Represents the type of data that will be exported.</typeparam>
public interface IExporter<TExportResult, in TSourceData>
    where TExportResult : IExportResult
{
    /// <summary>
    /// Exports data and returns the result of the export operation.
    /// </summary>
    /// <param name="sourceData">The data to be exported, which is processed during the export operation.</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the export.</returns>
    Task<TExportResult> Export( TSourceData sourceData );
}