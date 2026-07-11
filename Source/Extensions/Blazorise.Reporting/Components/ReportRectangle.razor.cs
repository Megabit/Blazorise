namespace Blazorise.Reporting;

/// <summary>
/// Declares a rectangular shape element in a report band.
/// </summary>
public partial class ReportRectangle : BaseReportElement
{
    #region Properties

    /// <inheritdoc />
    protected override ReportElementType ElementType => ReportElementType.Rectangle;

    #endregion
}