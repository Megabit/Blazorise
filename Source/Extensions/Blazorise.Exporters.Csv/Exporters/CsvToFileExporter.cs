using System.Text;
using System.Threading.Tasks;
using Blazorise.Export;

namespace Blazorise.Exporters.Csv;

/// <summary>
/// Exports DataGrid content to a CSV file using the specified file export options.
/// </summary>
public class CsvToFileTextExporter : TextExporterToFile<CsvFileExportOptions, ExportResult, TabularSourceData<string>>
{
    public CsvToFileTextExporter(CsvFileExportOptions options = null) : base(options) { }

    public override async Task<byte[]> GetDataForExport( TabularSourceData<string> sourceData )
    {
        var content = await CsvExportHelpers.GetDataForText( sourceData.Data, sourceData.ColumnNames, FileOptions );
        var bytes = Encoding.UTF8.GetBytes( content );
        return bytes;
    }
  
}
