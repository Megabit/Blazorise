using System;
using System.Collections.Generic;
using Blazorise;

namespace Blazorise.Reporting;

/// <summary>
/// Describes an image element placed on a report band.
/// </summary>
public sealed class ReportImageElementDefinition : ReportElementDefinition
{
    /// <inheritdoc />
    public override ReportElementType Type => ReportElementType.Image;

    /// <summary>
    /// Image source used by the element.
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// Defines how the image fits inside the element bounds.
    /// </summary>
    public ReportImageFit Fit { get; set; }

    /// <summary>
    /// Alternate text associated with the image.
    /// </summary>
    public string Text { get; set; }
}