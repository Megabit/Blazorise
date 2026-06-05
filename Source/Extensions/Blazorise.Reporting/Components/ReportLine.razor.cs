namespace Blazorise.Reporting;

/// <summary>
/// Declares a line element in a report band.
/// </summary>
public partial class ReportLine : ReportElementBase
{
    /// <inheritdoc />
    protected override ReportElementType ElementType => ReportElementType.Line;
}