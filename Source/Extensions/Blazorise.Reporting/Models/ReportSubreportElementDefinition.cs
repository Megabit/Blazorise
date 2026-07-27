namespace Blazorise.Reporting;

/// <summary>
/// Describes a nested report element rendered inside the current report.
/// </summary>
public sealed class ReportSubreportElementDefinition : ReportElementDefinition
{
    /// <inheritdoc />
    public override ReportElementType Type => ReportElementType.Subreport;

    /// <summary>
    /// Nested report definition rendered inside the element bounds.
    /// </summary>
    public ReportDefinition Report { get; set; }

    /// <summary>
    /// Parent data source name or field path used as the nested report data.
    /// </summary>
    public string DataSource { get; set; }

    internal ReportContext DeclarativeContext { get; set; }
}