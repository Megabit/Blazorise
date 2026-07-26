namespace Blazorise.Reporting;

/// <summary>
/// Defines time formatting for report values.
/// </summary>
public sealed class ReportTimeFormatDefinition : ReportTemporalFormatDefinition
{
    /// <summary>
    /// Initializes a new report time format definition.
    /// </summary>
    public ReportTimeFormatDefinition()
    {
        DateFormat = ReportDateFormat.ShortTime;
    }

    /// <inheritdoc />
    public override ReportFormatCategory Category => ReportFormatCategory.Time;
}