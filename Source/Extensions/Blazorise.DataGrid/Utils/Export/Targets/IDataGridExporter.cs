using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blazorise.DataGrid;


/// <summary>
/// Represents a DataGrid exporter that handles string-based (textual) cell values, such as CSV or plain text formats.
/// </summary>
public interface IDataGridExporterText<TExportResult> : IDataGridExporter<TExportResult,string>
where TExportResult : IExportResult
{
    
}

/// <summary>
/// Represents a DataGrid exporter that handles object-based (typed or binary) cell values for formats like Excel or BSON.
/// </summary>
public interface IDataGridExporterBinary<TExportResult> : IDataGridExporter<TExportResult,object>
where TExportResult : IExportResult
{
    
}

/// <summary>
/// 🚫 This is an internal base interface for DataGrid exporters and should not be implemented directly.
/// Use <see cref="IDataGridExporterText{TExportResult}"/> or <see cref="IDataGridExporterBinary{TExportResult}"/> instead.
/// Defines a generic interface for exporting DataGrid content using a specified cell value type and export result type.
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]

public interface IDataGridExporter<TExportResult, TDataType>
where TExportResult: IExportResult
{
     Task<TExportResult> Export( List<List<TDataType>> data, List<string> columnNames );

}



