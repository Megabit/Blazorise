namespace Blazorise.Exporters;

/// <summary>
/// Base class for exporting  content as text (string-based) to a file using specified file export options.
/// </summary>
public abstract class TextExporterToFile<TOptions, TExportResult, TSourceData> : ExporterToFile<TOptions, TExportResult, TSourceData>, ITextExporter<TExportResult, TSourceData>
    where TOptions : IFileExportOptions, new()
    where TExportResult : IExportResult, new()
    where TSourceData : IExportableData<string>
{
    /// <summary>
    /// Initializes a new instance of the TextExporterToFile class with specified options.
    /// </summary>
    /// <param name="options">Specifies the configuration settings for the text export functionality.</param>
    protected TextExporterToFile( TOptions options ) 
        : base( options )
    {
    }
}