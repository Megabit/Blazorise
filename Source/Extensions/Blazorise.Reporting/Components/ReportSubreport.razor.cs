using Microsoft.AspNetCore.Components;

namespace Blazorise.Reporting;

/// <summary>
/// Declares a nested report element in a report band.
/// </summary>
public partial class ReportSubreport
{
    private readonly ReportContext context = new();

    /// <inheritdoc />
    protected override ReportElementType ElementType => ReportElementType.Subreport;

    /// <inheritdoc />
    protected override void OnAfterRender( bool firstRender )
    {
        SyncNestedReportDefinition();
    }

    /// <inheritdoc />
    protected override ReportElementDefinition BuildDefinition()
    {
        ReportSubreportElementDefinition definition = (ReportSubreportElementDefinition)base.BuildDefinition();
        definition.Report = ResolveNestedReportDefinition();
        definition.DeclarativeContext = Report is null && ChildContent is not null ? context : null;
        definition.DataSource = DataSource;

        return definition;
    }

    private void SyncNestedReportDefinition()
    {
        if ( Definition is ReportSubreportElementDefinition subreportDefinition )
        {
            subreportDefinition.Report = ResolveNestedReportDefinition();
            subreportDefinition.DeclarativeContext = Report is null && ChildContent is not null ? context : null;
        }
    }

    private ReportDefinition ResolveNestedReportDefinition()
    {
        ReportDefinition definition = Report is not null
            ? ReportContext.CloneDefinition( Report )
            : ChildContent is not null
                ? context.BuildDefinition()
                : Internal.ReportDefinitionHelper.CreateDefaultSubreportDefinition( Name );

        if ( definition is not null && string.IsNullOrWhiteSpace( definition.Name ) )
            definition.Name = string.IsNullOrWhiteSpace( Name ) ? "Subreport" : Name;

        return definition;
    }

    internal ReportContext Context => context;

    /// <summary>
    /// Nested report definition rendered inside the element bounds.
    /// </summary>
    [Parameter] public ReportDefinition Report { get; set; }

    /// <summary>
    /// Parent data source name or field path used as the nested report data.
    /// </summary>
    [Parameter] public string DataSource { get; set; }

    /// <summary>
    /// Declarative report bands placed inside the subreport.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }
}