namespace Blazorise.Reporting;

/// <summary>
/// Declares an explicit page break in a report band.
/// </summary>
public partial class ReportPageBreak : BaseReportElement
{
    #region Properties

    /// <inheritdoc />
    protected override ReportElementType ElementType => ReportElementType.PageBreak;

    #endregion
}