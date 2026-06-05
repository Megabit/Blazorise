namespace Blazorise.Reporting;

/// <summary>
/// Declares an explicit page break in a report band.
/// </summary>
public partial class ReportPageBreak : ReportElementBase
{
    /// <inheritdoc />
    protected override ReportElementType ElementType => ReportElementType.PageBreak;
}