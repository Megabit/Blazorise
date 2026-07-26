namespace Blazorise.Reporting;

/// <summary>
/// Defines date formatting for report values.
/// </summary>
public sealed class ReportDateFormatDefinition : ReportTemporalFormatDefinition
{
    /// <summary>
    /// Initializes a new report date format definition.
    /// </summary>
    public ReportDateFormatDefinition()
    {
        DateFormat = ReportDateFormat.ShortDate;
    }

    /// <inheritdoc />
    public override ReportFormatCategory Category => ReportFormatCategory.Date;
}