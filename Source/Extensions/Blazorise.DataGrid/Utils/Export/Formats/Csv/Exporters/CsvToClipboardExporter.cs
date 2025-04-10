using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blazorise.DataGrid;

/// <summary>
/// Provides functionality to export DataGrid content in CSV format directly to the system clipboard.
/// </summary>
public class CsvToClipboardExporter : DataGridExporterToClipboardText<CsvClipboardExportOptions, ExportResult>
{
    public CsvToClipboardExporter(CsvClipboardExportOptions options = null) : base(options) { }

    public override async Task<string> GetDataForExport( List<List<string>> data, List<string> columnNames )
    {
        var content = await CsvExportHelpers.GetDataForText( data, columnNames, ClipboardOptions );
        return content;
    }

    
}