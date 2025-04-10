using System.Collections.Generic;
using System.Threading.Tasks;
namespace Blazorise.DataGrid;

/// <summary>
/// Exports DataGrid content to a CSV-formatted string using the specified text export options.
/// </summary>
public class CsvToTextExporter : DataGridExporterToText<CsvTextExportOptions, TextExportResult>
{
    public CsvToTextExporter(CsvTextExportOptions options = null) : base(options) { }

    public override async Task<TextExportResult> Export( List<List<string>> data, List<string> columnNames )
    {
        var content = await CsvExportHelpers.GetDataForText( data, columnNames, TextOptions );
        return new TextExportResult {IsSuccess = true, Text = content};    
    }
}
