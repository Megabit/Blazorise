using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Modules;

namespace Blazorise.DataGrid;

/// <summary>
/// Base class for exporting DataGrid content as text (string-based) to the clipboard using specified options.
/// </summary>
public abstract class DataGridExporterToClipboardText<TOptions,TExportResult>:DataGridExporterToClipboard<TOptions,TExportResult, string>, IDataGridExporterText<TExportResult>
where TOptions: IClipboardExportOptions, new()
where TExportResult: IExportResult, new()
{
    protected DataGridExporterToClipboardText(TOptions options) : base(options)
    {
    }
}

/// <summary>
/// Base class for exporting DataGrid content in binary format to the clipboard using specified options.
/// </summary>
public abstract class DataGridExporterToClipboardBinary<TOptions,TExportResult>
:DataGridExporterToClipboard<TOptions,TExportResult,object>, IDataGridExporterBinary<TExportResult>
where TOptions: IClipboardExportOptions, new()
where TExportResult: IExportResult, new()
{
    protected DataGridExporterToClipboardBinary(TOptions options) : base(options)
    {
    }
}


/// <summary>
/// 🚫 This is an internal base class for clipboard export targets.  
/// Do not use this class directly.  
/// Instead, use a specialized implementation such as <see cref="DataGridExporterToClipboardText{TOptions, TExportResult}"/>.
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class DataGridExporterToClipboard<TOptions, TExportResult, TCellValue>:IDataGridExporter<TExportResult,TCellValue>, IClipboardExportTarget
where TOptions: IClipboardExportOptions, new()
where TExportResult : IExportResult, new()
{
    protected DataGridExporterToClipboard( TOptions options )
        => ClipboardOptions = options ?? new();
    public IJSUtilitiesModule JSUtilitiesModule { get; set; }

    public async Task<TExportResult> Export( List<List<TCellValue>> data, List<string> columnNames )
    {
        var text = await GetDataForExport( data, columnNames );
        await JSUtilitiesModule.CopyStringToClipboard( text );
        
        return  await GetExportResult( true );

    }
    
    public abstract Task< string> GetDataForExport( List<List<TCellValue>> data, List<string> columnNames );

    public virtual Task<TExportResult> GetExportResult(bool success)
    {
        var res= new TExportResult { IsSuccess = success};
        return Task.FromResult( res );
    }

    
    
    
    public TOptions ClipboardOptions { get; init; }

}