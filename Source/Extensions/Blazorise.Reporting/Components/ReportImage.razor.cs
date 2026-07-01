using Microsoft.AspNetCore.Components;

namespace Blazorise.Reporting;

/// <summary>
/// Declares an image element in a report band.
/// </summary>
public partial class ReportImage : BaseReportElement
{
    /// <inheritdoc />
    protected override ReportElementType ElementType => ReportElementType.Image;

    /// <inheritdoc />
    protected override ReportElementDefinition BuildDefinition()
    {
        ReportImageElementDefinition definition = (ReportImageElementDefinition)base.BuildDefinition();
        definition.Source = Source;
        definition.Text = Alt;

        return definition;
    }

    /// <summary>
    /// Image source URL or data URI rendered by the element.
    /// </summary>
    [Parameter] public string Source { get; set; }

    /// <summary>
    /// Alternate text associated with the image element.
    /// </summary>
    [Parameter] public string Alt { get; set; }
}