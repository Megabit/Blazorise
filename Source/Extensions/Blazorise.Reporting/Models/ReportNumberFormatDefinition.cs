namespace Blazorise.Reporting;

/// <summary>
/// Defines number formatting for report values.
/// </summary>
public sealed class ReportNumberFormatDefinition : ReportNumericFormatDefinition
{
    /// <inheritdoc />
    public override ReportFormatCategory Category => ReportFormatCategory.Number;

    /// <summary>
    /// Indicates whether numbers should use group separators.
    /// </summary>
    public bool UseThousandsSeparator { get; set; } = true;
}