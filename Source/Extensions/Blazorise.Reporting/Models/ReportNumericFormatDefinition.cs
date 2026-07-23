namespace Blazorise.Reporting;

/// <summary>
/// Defines common formatting options for numeric report values.
/// </summary>
public abstract class ReportNumericFormatDefinition : ReportFormatDefinition
{
    /// <summary>
    /// Decimal places used by numeric formats.
    /// </summary>
    public int? DecimalPlaces { get; set; }

    /// <summary>
    /// Negative number display style.
    /// </summary>
    public ReportNegativeNumberFormat NegativeNumberFormat { get; set; }
}