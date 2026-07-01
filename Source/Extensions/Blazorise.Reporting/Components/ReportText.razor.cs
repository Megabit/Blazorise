using Microsoft.AspNetCore.Components;

namespace Blazorise.Reporting;

/// <summary>
/// Declares a static text element in a report band.
/// </summary>
public partial class ReportText : BaseReportTextElement
{
    /// <inheritdoc />
    protected override ReportElementType ElementType => ReportElementType.Text;

    /// <inheritdoc />
    protected override ReportElementDefinition BuildDefinition()
    {
        ReportTextElementDefinition definition = (ReportTextElementDefinition)base.BuildDefinition();
        definition.Text = Text;

        return definition;
    }

    /// <summary>
    /// Text content rendered by the element.
    /// </summary>
    [Parameter] public string Text { get; set; }
}