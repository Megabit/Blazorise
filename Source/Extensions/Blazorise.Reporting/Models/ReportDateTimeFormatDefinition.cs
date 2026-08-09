namespace Blazorise.Reporting;

/// <summary>
/// Defines date and time formatting for report values.
/// </summary>
public sealed class ReportDateTimeFormatDefinition : ReportTemporalFormatDefinition
{
    /// <summary>
    /// Initializes a new report date and time format definition.
    /// </summary>
    public ReportDateTimeFormatDefinition()
    {
        DateFormat = ReportDateFormat.ShortDateTime;
    }

    /// <inheritdoc />
    public override ReportFormatCategory Category => ReportFormatCategory.DateTime;
}