namespace Blazorise.Reporting;

/// <summary>
/// Defines plain text formatting for report values.
/// </summary>
public sealed class ReportTextFormatDefinition : ReportFormatDefinition
{
    /// <inheritdoc />
    public override ReportFormatCategory Category => ReportFormatCategory.Text;
}