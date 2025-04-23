using System.ComponentModel;
using System.Threading.Tasks;

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

    public JSExportersModule JsExportersModule { get; set; }

    public abstract Task<byte[]> GetDataForExport( TSourceData dataSource );

    public virtual Task<TExportResult> GetExportResult( int exportToFileResult )
    {
        var res = new TExportResult { Success = exportToFileResult == 1 };
        return Task.FromResult( res );
    }

    public async Task<TExportResult> Export( TSourceData dataSource )
    {
        var bytes = await GetDataForExport( dataSource );
        var res = await JsExportersModule.ExportToFile( bytes, FileOptions.FileName, FileOptions.MimeType );

        return await GetExportResult( res );
    }

    public TOptions FileOptions { get; init; }
}