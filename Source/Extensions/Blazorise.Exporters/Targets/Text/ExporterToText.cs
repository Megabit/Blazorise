namespace Blazorise.Exporters;

/// <summary>
///
/// </summary>
public abstract class ExporterToText<TOptions,TExportResult, TSourceData>
where TOptions: ITextExportOptions, new()
where TExportResult: IExportResult, new()
{
    protected ExporterToText( TOptions options )
        => TextOptions = options ?? new();

    public abstract  Task<TExportResult> Export( TSourceData sourceData );
    
    public TOptions TextOptions { get; init; }

}


