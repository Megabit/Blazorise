#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Declares a static text element in a report band.
/// </summary>
public partial class ReportText : BaseReportTextElement
{
    #region Methods

    /// <inheritdoc />
    protected override ReportElementDefinition BuildDefinition()
    {
        ReportTextElementDefinition definition = (ReportTextElementDefinition)base.BuildDefinition();
        definition.Text = Text;

        return definition;
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    protected override ReportElementType ElementType => ReportElementType.Text;

    /// <summary>
    /// Text content rendered by the element.
    /// </summary>
    [Parameter] public string Text { get; set; }

    #endregion
}