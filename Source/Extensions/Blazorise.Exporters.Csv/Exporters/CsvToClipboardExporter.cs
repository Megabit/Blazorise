using System.Threading.Tasks;

namespace Blazorise.Exporters.Csv;

/// <summary>
/// Provides functionality to export DataGrid content in CSV format directly to the system clipboard.
/// </summary>
public class CsvToClipboardExporter : TextExporterToClipboard<CsvClipboardExportOptions, ExportResult, TabularSourceData<string>>
{
    /// <summary>
    /// Initializes a new instance of the CsvToClipboardExporter class, allowing for CSV data to be exported to the
    /// clipboard.
    /// </summary>
    /// <param name="options">Specifies the settings for exporting CSV data.</param>
    public CsvToClipboardExporter( CsvClipboardExportOptions options = null )
        : base( options )
    {
    }

    /// <summary>
    /// Retrieves data formatted for export as a string, specifically in CSV format.
    /// </summary>
    /// <param name="sourceData">Contains the data and column names needed for generating the export content.</param>
    /// <returns>A string representing the formatted data ready for export.</returns>
    public override async Task<string> GetDataForExport( TabularSourceData<string> sourceData )
    {
        var content = await CsvExportHelpers.GetDataForText( sourceData.Data, sourceData.ColumnNames, ClipboardOptions );

        return content;
    }
}