using System.Text;

namespace Blazorise.Exporters.Csv;

/// <summary>
/// Exports DataGrid content to a CSV file using the specified file export options.
/// </summary>
public class CsvToFileExporter : TextExporterToFile<CsvFileExportOptions, ExportResult, TabularSourceData<string>>
{
    public CsvToFileExporter(CsvFileExportOptions options = null) : base(options) { }

    public override async Task<byte[]> GetDataForExport( TabularSourceData<string> sourceData )
    {
        var content = await CsvExportHelpers.GetDataForText( sourceData.Data, sourceData.ColumnNames, FileOptions );
        var bytes = Encoding.UTF8.GetBytes( content );
        return bytes;
    }
  
}
