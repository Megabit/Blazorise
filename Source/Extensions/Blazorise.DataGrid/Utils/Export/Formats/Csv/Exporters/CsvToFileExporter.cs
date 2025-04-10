using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Blazorise.DataGrid;

/// <summary>
/// Exports DataGrid content to a CSV file using the specified file export options.
/// </summary>
public class CsvToFileExporter : DataGridExporterToFileText<CsvFileExportOptions, ExportResult >
{
    public CsvToFileExporter(CsvFileExportOptions options = null) : base(options) { }

    public override async Task<byte[]> GetDataForExport( List<List<string>> data, List<string> columnNames )
    {
        var content = await CsvExportHelpers.GetDataForText( data, columnNames, FileOptions );
        var bytes = Encoding.UTF8.GetBytes( content );
        return bytes;
    }
}
