using System.Threading.Tasks;

namespace Blazorise.Exporters.Csv;

/// <summary>
/// Exports DataGrid content to a CSV-formatted string using the specified text export options.
/// </summary>
public class CsvToTextExporter : ExporterToText<CsvTextExportOptions, TextExportResult, TabularSourceData<string>>
{
    /// <summary>
    /// Initializes a new instance of the CsvToTextExporter class, allowing for the export of CSV data to text format.
    /// </summary>
    /// <param name="options">Specifies the configuration settings for the CSV to text export process.</param>
    public CsvToTextExporter( CsvTextExportOptions options = null )
        : base( options )
    {
    }

    /// <summary>
    /// Exports data from a tabular source into a text format asynchronously.
    /// </summary>
    /// <param name="sourceData">Contains the data and column names to be exported in the text format.</param>
    /// <returns>Returns a result indicating the success of the export along with the generated text content.</returns>
    public override async Task<TextExportResult> Export( TabularSourceData<string> sourceData )
    {
        var content = await CsvExportHelpers.GetDataForText( sourceData.Data, sourceData.ColumnNames, TextOptions );

        return new TextExportResult
        {
            Success = true,
            Text = content,
        };
    }
}