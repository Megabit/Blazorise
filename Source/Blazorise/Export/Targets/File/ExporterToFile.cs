using System.ComponentModel;
using System.Threading.Tasks;
using Blazorise.Export;
using Blazorise.Modules;

namespace Blazorise.Export;

/// <summary>
/// Base class for exporting  content as text (string-based) to a file using specified file export options.
/// </summary>
public abstract class ExporterToFileText<TOptions,TExportResult, TSourceData>
:ExporterToFile<TOptions,TExportResult, TSourceData>, IExporterText<TExportResult,TSourceData>
where TOptions: IFileExportOptions, new()
where TExportResult: IExportResult, new()
where TSourceData: IExportableData<string>
{
    protected ExporterToFileText(TOptions options) : base(options)
    {
    }
}

/// <summary>
/// Base class for exporting  content in binary format to a file using specified file export options.
/// </summary>
public abstract class ExporterToFileBinary<TOptions,TExportResult,TSourceData>
:ExporterToFile<TOptions,TExportResult,TSourceData>, IExporterBinary<TExportResult, TSourceData>
where TOptions: IFileExportOptions, new()
where TExportResult: IExportResult, new()
where TSourceData: IExportableData<object>

{
    protected ExporterToFileBinary(TOptions options) : base(options)
    {
    }
}


/// <summary>
/// 🚫 Internal base class. Do not use directly. Inherit from <see cref="ExporterToFileText{TOptions,TExportResult}"/> or <see cref="ExporterToFileBinary{TOptions,TExportResult}"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class ExporterToFile<TOptions,TExportResult, TSourceData>:
IFileExportTarget 
where TOptions: IFileExportOptions, new()
where TExportResult: IExportResult, new()
{
    protected ExporterToFile(TOptions options)
        => FileOptions = options ?? new();
    public IJSUtilitiesModule JSUtilitiesModule { get; set; }

    public abstract Task<byte[]> GetDataForExport( TSourceData dataSource );

    public virtual Task<TExportResult> GetExportResult(int exportToFileResult)
    {
        var res= new TExportResult { IsSuccess = exportToFileResult == 1 };
        return Task.FromResult( res );
    }

    public async Task<TExportResult> Export( TSourceData dataSource )
    {
        var bytes = await GetDataForExport( dataSource );
        var res =  await JSUtilitiesModule.ExportToFile(bytes, FileOptions.FileName, FileOptions.MimeType );
        
        return  await GetExportResult( res );
    }
    public TOptions FileOptions { get; init; }
}


