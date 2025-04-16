namespace Blazorise.Exporters.Csv;

/// <summary>
/// Exports DataGrid content to a CSV-formatted string using the specified text export options.
/// </summary>
public class CsvToTextExporter : ExporterToText<CsvTextExportOptions, TextExportResult, TabularSourceData<string>>
{
    public CsvToTextExporter(CsvTextExportOptions options = null) : base(options) { }

    public override async Task<TextExportResult> Export( TabularSourceData<string> sourceData )
    {
        var content = await CsvExportHelpers.GetDataForText( sourceData.Data, sourceData.ColumnNames, TextOptions );
        return new TextExportResult {Success = true, Text = content};    
    }
}
