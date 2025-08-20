using System.Text;
using System.Threading.Tasks;

namespace Blazorise.Exporters.Csv;

/// <summary>
/// Exports DataGrid content to a CSV file using the specified file export options.
/// </summary>
public class CsvToFileExporter : TextExporterToFile<CsvFileExportOptions, ExportResult, TabularSourceData<string>>
{
    /// <summary>
    /// Initializes a new instance of the CsvToFileExporter class, allowing for CSV file export functionality.
    /// </summary>
    /// <param name="options">Specifies the configuration settings for exporting CSV files.</param>
    public CsvToFileExporter( CsvFileExportOptions options = null )
        : base( options )
    {
    }

    /// <summary>
    /// Exports data from a tabular source into a byte array formatted as CSV.
    /// </summary>
    /// <param name="sourceData">Contains the data and column names to be exported.</param>
    /// <returns>A byte array representing the CSV formatted data.</returns>
    public override async Task<byte[]> GetDataForExport( TabularSourceData<string> sourceData )
    {
        var content = await CsvExportHelpers.GetDataForText( sourceData.Data, sourceData.ColumnNames, FileOptions );
        var bytes = Encoding.UTF8.GetBytes( content );

        return bytes;
    }
}