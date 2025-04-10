using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace Blazorise.DataGrid;

/// <summary>
/// Base class for exporting DataGrid content as text (string-based) to a file using specified file export options.
/// </summary>
public abstract class DataGridExporterToFileText<TOptions,TExportResult>:DataGridExporterToFile<TOptions,TExportResult, string>, IDataGridExporterText<TExportResult>
where TOptions: IFileExportOptions, new()
where TExportResult: IExportResult, new()
{
    protected DataGridExporterToFileText(TOptions options) : base(options)
    {
    }
}

/// <summary>
/// Base class for exporting DataGrid content in binary format to a file using specified file export options.
/// </summary>
public abstract class DataGridExporterToFileBinary<TOptions,TExportResult>
:DataGridExporterToFile<TOptions,TExportResult,object>, IDataGridExporterBinary<TExportResult>
where TOptions: IFileExportOptions, new()
where TExportResult: IExportResult, new()
{
    protected DataGridExporterToFileBinary(TOptions options) : base(options)
    {
    }
}


/// <summary>
/// 🚫 Internal base class. Do not use directly. Inherit from <see cref="DataGridExporterToFileText{TOptions,TExportResult}"/> or <see cref="DataGridExporterToFileBinary{TOptions,TExportResult}"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class DataGridExporterToFile<TOptions,TExportResult, TCellValue>:
IFileExportTarget 
where TOptions: IFileExportOptions, new()
where TExportResult: IExportResult, new()
{
    protected DataGridExporterToFile(TOptions options)
        => FileOptions = options ?? new();
    public JSDataGridModule JsDataGridModule { get; set; }

    public abstract Task<byte[]> GetDataForExport( List<List<TCellValue>> data, List<string> columnNames );

    public virtual Task<TExportResult> GetExportResult(int exportToFileResult)
    {
        var res= new TExportResult { IsSuccess = exportToFileResult == 1 };
        return Task.FromResult( res );
    }

    public async Task<TExportResult> Export( List<List<TCellValue>> data, List<string> columnNames )
    {
        var bytes = await GetDataForExport( data, columnNames );
        var res =  await JsDataGridModule.ExportToFile(bytes, FileOptions.FileName, FileOptions.MimeType );
        
        return  await GetExportResult( res );
    }
    public TOptions FileOptions { get; init; }
}


