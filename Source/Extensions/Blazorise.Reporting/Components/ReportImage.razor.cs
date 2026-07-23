#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Declares an image element in a report band.
/// </summary>
public partial class ReportImage : BaseReportElement
{
    #region Methods

    /// <inheritdoc />
    protected override ReportElementDefinition BuildDefinition()
    {
        ReportImageElementDefinition definition = (ReportImageElementDefinition)base.BuildDefinition();
        definition.Source = Source;
        definition.Fit = Fit;
        definition.Text = Alt;

        return definition;
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    protected override ReportElementType ElementType => ReportElementType.Image;

    /// <summary>
    /// Image source URL or data URI rendered by the element.
    /// </summary>
    [Parameter] public string Source { get; set; }

    /// <summary>
    /// Defines how the image fits inside the element bounds.
    /// </summary>
    [Parameter] public ReportImageFit Fit { get; set; }

    /// <summary>
    /// Alternate text associated with the image element.
    /// </summary>
    [Parameter] public string Alt { get; set; }

    #endregion
}