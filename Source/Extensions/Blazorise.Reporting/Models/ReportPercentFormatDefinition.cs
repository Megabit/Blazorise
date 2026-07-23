namespace Blazorise.Reporting;

/// <summary>
/// Defines percent formatting for report values.
/// </summary>
public sealed class ReportPercentFormatDefinition : ReportNumericFormatDefinition
{
    /// <inheritdoc />
    public override ReportFormatCategory Category => ReportFormatCategory.Percent;
}