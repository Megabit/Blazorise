using System.ComponentModel;
using System.Threading.Tasks;

namespace Blazorise.Exporters;

/// <summary>
/// Base class for exporting  content as text (string-based) to a clipboard using specified clipboard export options.
/// </summary>
public abstract class TextExporterToClipboard<TOptions, TExportResult, TSourceData> : ExporterToClipboard<TOptions, TExportResult, TSourceData>, ITextExporter<TExportResult, TSourceData>
    where TOptions : IClipboardExportOptions, new()
    where TExportResult : IExportResult, new()
    where TSourceData : IExportableData<string>
{
    protected TextExporterToClipboard( TOptions options ) : base( options )
    {
    }
}

/// <summary>
/// Base class for exporting  content in binary format to a clipboard using specified clipboard export options.
/// </summary>
public abstract class BinaryExporterToClipboard<TOptions, TExportResult, TSourceData> : ExporterToClipboard<TOptions, TExportResult, TSourceData>, IBinaryExporter<TExportResult, TSourceData>
    where TOptions : IClipboardExportOptions, new()
    where TExportResult : IExportResult, new()
    where TSourceData : IExportableData<object>
{
    protected BinaryExporterToClipboard( TOptions options ) : base( options )
    {
    }
}


/// <summary>
/// 🚫 Internal base class. Do not use directly. Inherit from <see cref="ExporterToClipboardText{TOptions,TExportResult}"/> or <see cref="ExporterToClipboardBinary{TOptions,TExportResult}"/>.
/// </summary>
[EditorBrowsable( EditorBrowsableState.Never )]
public abstract class ExporterToClipboard<TOptions, TExportResult, TSourceData> : IExporterWithJsModule
    where TOptions : IClipboardExportOptions, new()
    where TExportResult : IExportResult, new()
{
    protected ExporterToClipboard( TOptions options ) => ClipboardOptions = options ?? new();

    public JSExportersModule JsExportersModule { get; set; }

    public async Task<TExportResult> Export( TSourceData sourceData )
    {
        var text = await GetDataForExport( sourceData );
        await JsExportersModule.CopyStringToClipboard( text );

        return await GetExportResult( true );

    }

    public abstract Task<string> GetDataForExport( TSourceData sourceData );

    public virtual Task<TExportResult> GetExportResult( bool success )
    {
        var res = new TExportResult { Success = success };
        return Task.FromResult( res );
    }

    public TOptions ClipboardOptions { get; init; }
}