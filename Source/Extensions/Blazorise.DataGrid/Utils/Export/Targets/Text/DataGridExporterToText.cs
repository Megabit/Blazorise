using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blazorise.DataGrid;

/// <summary>
/// Base class for exporting DataGrid content as plain text using the specified text export options.
/// Intended for formats like CSV or TSV where cell values are represented as strings.
/// </summary>
public abstract class DataGridExporterToText<TOptions, TExportResult>: IDataGridExporter<TExportResult,string>
    where TOptions : ITextExportOptions, new()
    where TExportResult: ITextExportResult
{
    
    protected DataGridExporterToText( TOptions options )
        => TextOptions = options ?? new();
    public TOptions TextOptions { get; init; }

    public abstract Task<TExportResult> Export( List<List<string>> data, List<string> columnNames );
}


