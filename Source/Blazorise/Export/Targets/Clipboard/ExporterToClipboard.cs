using System.ComponentModel;
using System.Threading.Tasks;
using Blazorise.Export;
using Blazorise.Modules;

namespace Blazorise.Export;

/// <summary>
/// Base class for exporting  content as text (string-based) to a clipboard using specified clipboard export options.
/// </summary>
public abstract class ExporterToClipboardText<TOptions,TExportResult, TSourceData>
:ExporterToClipboard<TOptions,TExportResult, TSourceData>, IExporterText<TExportResult,TSourceData>
where TOptions: IClipboardExportOptions, new()
where TExportResult: IExportResult, new()
where TSourceData: IExportableData<string>
{
    protected ExporterToClipboardText(TOptions options) : base(options)
    {
    }

   
}

/// <summary>
/// Base class for exporting  content in binary format to a clipboard using specified clipboard export options.
/// </summary>
public abstract class ExporterToClipboardBinary<TOptions,TExportResult,TSourceData>
:ExporterToClipboard<TOptions,TExportResult,TSourceData>, IExporterBinary<TExportResult, TSourceData>
where TOptions: IClipboardExportOptions, new()
where TExportResult: IExportResult, new()
where TSourceData: IExportableData<object>

{
    protected ExporterToClipboardBinary(TOptions options) : base(options)
    {
    }
}


/// <summary>
/// 🚫 Internal base class. Do not use directly. Inherit from <see cref="ExporterToClipboardText{TOptions,TExportResult}"/> or <see cref="ExporterToClipboardBinary{TOptions,TExportResult}"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class ExporterToClipboard<TOptions,TExportResult, TSourceData>:
IClipboardExportTarget 
where TOptions: IClipboardExportOptions, new()
where TExportResult: IExportResult, new()
{
    protected ExporterToClipboard( TOptions options )
        => ClipboardOptions = options ?? new();
    public IJSUtilitiesModule JSUtilitiesModule { get; set; }

    public async Task<TExportResult> Export( TSourceData  sourceData)
    {
        var text = await GetDataForExport( sourceData );
        await JSUtilitiesModule.CopyStringToClipboard( text );
        
        return  await GetExportResult( true );

    }
    
    public abstract Task< string> GetDataForExport( TSourceData sourceData );

    public virtual Task<TExportResult> GetExportResult(bool success)
    {
        var res= new TExportResult { IsSuccess = success};
        return Task.FromResult( res );
    }

    
    
    
    public TOptions ClipboardOptions { get; init; }

}


