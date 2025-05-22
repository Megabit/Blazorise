using System.ComponentModel;
using System.Threading.Tasks;
using Blazorise.Modules;

namespace Blazorise.Exporters;

/// <summary>
/// 🚫 Internal base class. Do not use directly. Inherit from <see cref="ExporterToFileText{TOptions,TExportResult}"/> or <see cref="ExporterToFileBinary{TOptions,TExportResult}"/>.
/// </summary>
[EditorBrowsable( EditorBrowsableState.Never )]
public abstract class ExporterToFile<TOptions, TExportResult, TSourceData> : IExporterWithJsModule
    where TOptions : IFileExportOptions, new()
    where TExportResult : IExportResult, new()
{
    /// <summary>
    /// Initializes a new instance of the ExporterToFile class with specified options for file export.
    /// </summary>
    /// <param name="options">Specifies the configuration settings for the file export process.</param>
    protected ExporterToFile( TOptions options )
    {
        FileOptions = options ?? new();
    }

    public IJSUtilitiesModule JSUtilitiesModule { get; set; }

    public abstract Task<byte[]> GetDataForExport( TSourceData dataSource );

    public virtual Task<TExportResult> GetExportResult( string[] exportToFileErrors)
    {
        var result = new TExportResult
        {
            Errors = exportToFileErrors
        };

        return Task.FromResult( result );
    }

    public async Task<TExportResult> Export( TSourceData dataSource )
    {
        var bytes = await GetDataForExport( dataSource );

        var result = await JSUtilitiesModule.ExportToFile( bytes, FileOptions.FileName, FileOptions.MimeType );

        return await GetExportResult( result );
    }

    public TOptions FileOptions { get; init; }
}