namespace Blazorise.Reporting;

/// <summary>
/// Defines custom .NET formatting for report values.
/// </summary>
public sealed class ReportCustomFormatDefinition : ReportFormatDefinition
{
    /// <inheritdoc />
    public override ReportFormatCategory Category => ReportFormatCategory.Custom;

    /// <summary>
    /// Custom .NET format string.
    /// </summary>
    public string Format { get; set; }
}