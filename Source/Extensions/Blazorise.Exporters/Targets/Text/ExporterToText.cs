using System.Threading.Tasks;

namespace Blazorise.Exporters;

/// <summary>
/// An abstract class for exporting data to text format, requiring specific options and result types.
/// </summary>
/// <typeparam name="TOptions">Specifies the options for text export, ensuring they conform to a defined interface.</typeparam>
/// <typeparam name="TExportResult">Defines the type of result produced from the export operation, adhering to a specific interface.</typeparam>
/// <typeparam name="TSourceData">Represents the type of data being exported to text format.</typeparam>
public abstract class ExporterToText<TOptions, TExportResult, TSourceData>
    where TOptions : ITextExportOptions, new()
    where TExportResult : IExportResult, new()
{
    /// <summary>
    /// Initializes a new instance of the ExporterToText class with specified options for text export.
    /// </summary>
    /// <param name="options">Specifies the configuration settings for the text export process.</param>
    protected ExporterToText( TOptions options )
        => TextOptions = options ?? new();

    /// <summary>
    /// Exports data from a specified source and returns the result asynchronously.
    /// </summary>
    /// <param name="sourceData">The input data that will be processed and exported.</param>
    /// <returns>An asynchronous task that yields the result of the export operation.</returns>
    public abstract Task<TExportResult> Export( TSourceData sourceData );

    /// <summary>
    /// Represents options for text formatting or styling.
    /// </summary>
    public TOptions TextOptions { get; init; }
}