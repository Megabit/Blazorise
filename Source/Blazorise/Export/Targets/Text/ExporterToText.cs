using System.Threading.Tasks;
using Blazorise.Modules;

namespace Blazorise.Export;

/// <summary>
///
/// </summary>
public abstract class ExporterToText<TOptions,TExportResult, TSourceData>

where TOptions: ITextExportOptions, new()
where TExportResult: IExportResult, new()
{
    protected ExporterToText( TOptions options )
        => TextOptions = options ?? new();
    public IJSUtilitiesModule JSUtilitiesModule { get; set; }

    public abstract  Task<TExportResult> Export( TSourceData sourceData );
    
    public TOptions TextOptions { get; init; }

}


