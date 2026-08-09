namespace Blazorise.Reporting;

/// <summary>
/// Defines boolean formatting for report values.
/// </summary>
public sealed class ReportBooleanFormatDefinition : ReportFormatDefinition
{
    /// <inheritdoc />
    public override ReportFormatCategory Category => ReportFormatCategory.Boolean;

    /// <summary>
    /// Text displayed for boolean true values.
    /// </summary>
    public string TrueText { get; set; }

    /// <summary>
    /// Text displayed for boolean false values.
    /// </summary>
    public string FalseText { get; set; }
}