namespace Blazorise.Reporting;

/// <summary>
/// Defines common formatting options for date and time report values.
/// </summary>
public abstract class ReportTemporalFormatDefinition : ReportFormatDefinition
{
    /// <summary>
    /// Date and time display style.
    /// </summary>
    public ReportDateFormat DateFormat { get; set; }
}