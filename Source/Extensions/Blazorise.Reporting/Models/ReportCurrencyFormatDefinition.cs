namespace Blazorise.Reporting;

/// <summary>
/// Defines currency formatting for report values.
/// </summary>
public sealed class ReportCurrencyFormatDefinition : ReportNumericFormatDefinition
{
    /// <inheritdoc />
    public override ReportFormatCategory Category => ReportFormatCategory.Currency;

    /// <summary>
    /// Optional currency symbol override.
    /// </summary>
    public string CurrencySymbol { get; set; }
}