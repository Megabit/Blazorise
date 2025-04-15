using System.Threading.Tasks;

namespace Blazorise.Export;

/// <summary>
/// Represents a DataGrid exporter that handles string-based (textual) cell values, such as CSV or plain text formats.
/// </summary>
public interface ITextExporter<TExportResult, in TSourceData> : IExporter<TExportResult, TSourceData>
where TExportResult : IExportResult
where TSourceData: IExportableData<string>

{
    
}

/// <summary>
/// Represents a DataGrid exporter that handles object-based (typed or binary) cell values for formats like Excel or BSON.
/// </summary>
public interface IBinaryExporter<TExportResult, in TSourceData> : IExporter<TExportResult, TSourceData>
where TExportResult : IExportResult
where TSourceData: IExportableData<object>
{
    
}


/// <summary>
///  Generic interface for exporting data.
/// </summary>
/// <typeparam name="TExportResult"></typeparam>
/// <typeparam name="TSourceData"></typeparam>
public interface IExporter<TExportResult, in TSourceData>
where TExportResult: IExportResult
{
    /// <summary>
    /// Export method that takes a source data object and returns an export result.
    /// </summary>
    /// <param name="sourceData"></param>
    /// <returns></returns>
    Task<TExportResult> Export( TSourceData sourceData );

}
