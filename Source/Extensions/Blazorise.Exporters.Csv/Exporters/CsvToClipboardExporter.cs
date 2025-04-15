
using Blazorise.Export;

namespace Blazorise.Exporters.Csv;

/// <summary>
/// Provides functionality to export DataGrid content in CSV format directly to the system clipboard.
/// </summary>
public class CsvToClipboardExporter : TextExporterToClipboard<CsvClipboardExportOptions, ExportResult, TabularSourceData<string>>
{
    public CsvToClipboardExporter(CsvClipboardExportOptions options = null) : base(options) { }

    public  override async Task<string> GetDataForExport( TabularSourceData<string> sourceData )
    {
        var content = await CsvExportHelpers.GetDataForText( sourceData.Data, sourceData.ColumnNames, ClipboardOptions );
        return content;
    }
}