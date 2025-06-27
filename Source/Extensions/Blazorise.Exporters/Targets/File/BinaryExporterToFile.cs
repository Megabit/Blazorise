namespace Blazorise.Exporters;

/// <summary>
/// Base class for exporting  content in binary format to a file using specified file export options.
/// </summary>
public abstract class BinaryExporterToFile<TOptions, TExportResult, TSourceData> : ExporterToFile<TOptions, TExportResult, TSourceData>, IBinaryExporter<TExportResult, TSourceData>
    where TOptions : IFileExportOptions, new()
    where TExportResult : IExportResult, new()
    where TSourceData : IExportableData<object>
{
    /// <summary>
    /// Initializes a BinaryExporterToFile instance with specified options for exporting data in binary format.
    /// </summary>
    /// <param name="options">Specifies the configuration settings for the binary export process.</param>
    protected BinaryExporterToFile( TOptions options )
        : base( options )
    {
    }
}