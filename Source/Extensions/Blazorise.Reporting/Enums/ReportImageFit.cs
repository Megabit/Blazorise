namespace Blazorise.Reporting;

/// <summary>
/// Defines how an image should fit inside a report image element.
/// </summary>
public enum ReportImageFit
{
    /// <summary>
    /// Uses the default report image fit behavior.
    /// </summary>
    Default,

    /// <summary>
    /// Scales the image to fit inside the element while preserving its aspect ratio.
    /// </summary>
    Contain,

    /// <summary>
    /// Scales the image to cover the entire element while preserving its aspect ratio.
    /// </summary>
    Cover,

    /// <summary>
    /// Stretches the image to fill the entire element without preserving its aspect ratio.
    /// </summary>
    Fill,

    /// <summary>
    /// Keeps the image at its natural size.
    /// </summary>
    None,

    /// <summary>
    /// Keeps the image at its natural size unless it is larger than the element, then scales it down to fit.
    /// </summary>
    ScaleDown,
}