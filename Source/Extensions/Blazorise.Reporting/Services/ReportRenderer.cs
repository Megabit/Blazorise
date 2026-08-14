#region Using directives
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Licensing;
using Blazorise.Pdf;
using Blazorise.Reporting.Internal;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Default backend report renderer.
/// </summary>
public sealed class ReportRenderer : IReportRenderer
{
    #region Members

    private readonly ReportDataSourceResolver dataSourceResolver;

    private readonly IReportElementPluginRegistry elementPluginRegistry;

    private readonly BlazoriseLicenseChecker licenseChecker;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new report renderer.
    /// </summary>
    /// <param name="dataSourceProviderRegistry">The registered report data source providers.</param>
    /// <param name="elementPluginRegistry">The registered custom report element plugins.</param>
    /// <param name="licenseChecker">The license checker used to apply report row limits.</param>
    public ReportRenderer(
        IReportDataSourceProviderRegistry dataSourceProviderRegistry,
        IReportElementPluginRegistry elementPluginRegistry,
        BlazoriseLicenseChecker licenseChecker )
        : this( new ReportDataSourceResolver( dataSourceProviderRegistry ), elementPluginRegistry, licenseChecker )
    {
    }

    internal ReportRenderer(
        ReportDataSourceResolver dataSourceResolver,
        IReportElementPluginRegistry elementPluginRegistry,
        BlazoriseLicenseChecker licenseChecker )
    {
        this.dataSourceResolver = dataSourceResolver;
        this.elementPluginRegistry = elementPluginRegistry;
        this.licenseChecker = licenseChecker;
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    public async Task<PdfDocumentDefinition> RenderAsync( ReportDefinition definition, ReportRenderOptions options = null, CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull( definition );

        options ??= new();
        cancellationToken.ThrowIfCancellationRequested();

        ReportDefinition workingDefinition = ReportDefinitionHelper.EnsureDefinitionIds( ReportContext.CloneDefinition( definition ) );
        ReportDefinitionHelper.ApplyRowsLimit( workingDefinition, BlazoriseLicenseLimitsHelper.GetReportingRowsLimit( licenseChecker ) );

        await dataSourceResolver.ResolveAsync( workingDefinition, new()
        {
            DefaultData = options.DefaultData,
            DataSources = options.DataSources,
            Parameters = options.Parameters,
            LoadData = true,
            RequireData = true,
        }, cancellationToken );

        IReportElementPluginRegistry renderPluginRegistry = options.ElementPlugins is null
            ? elementPluginRegistry
            : new ReportElementPluginRegistry( ( elementPluginRegistry?.Plugins ?? [] ).Concat( options.ElementPlugins ) );

        return ReportPdfDocumentBuilder.Build( workingDefinition, options.DefaultData, renderPluginRegistry, cancellationToken );
    }

    #endregion
}