namespace Blazorise.Reporting;

/// <summary>
/// Declares a rectangular shape element in a report band.
/// </summary>
public partial class ReportRectangle : ReportElementBase
{
    /// <inheritdoc />
    protected override ReportElementType ElementType => ReportElementType.Rectangle;
}