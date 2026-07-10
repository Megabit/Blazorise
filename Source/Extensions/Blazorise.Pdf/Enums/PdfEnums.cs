namespace Blazorise.Pdf;

/// <summary>
/// Defines standard PDF page sizes.
/// </summary>
public enum PdfPageSize
{
    /// <summary>
    /// Custom page dimensions.
    /// </summary>
    Custom,

    /// <summary>
    /// ISO A4 page size.
    /// </summary>
    A4,

    /// <summary>
    /// US Letter page size.
    /// </summary>
    Letter,
}

/// <summary>
/// Defines PDF page orientation.
/// </summary>
public enum PdfOrientation
{
    /// <summary>
    /// Portrait page orientation.
    /// </summary>
    Portrait,

    /// <summary>
    /// Landscape page orientation.
    /// </summary>
    Landscape,
}

/// <summary>
/// Defines PDF element kinds.
/// </summary>
public enum PdfElementType
{
    /// <summary>
    /// Text element.
    /// </summary>
    Text,

    /// <summary>
    /// Image element.
    /// </summary>
    Image,

    /// <summary>
    /// Line element.
    /// </summary>
    Line,

    /// <summary>
    /// Rectangle element.
    /// </summary>
    Rectangle,

    /// <summary>
    /// Table element.
    /// </summary>
    Table,
}

/// <summary>
/// Defines PDF border styles.
/// </summary>
public enum PdfBorderStyle
{
    /// <summary>
    /// Solid border.
    /// </summary>
    Solid,

    /// <summary>
    /// Dashed border.
    /// </summary>
    Dashed,

    /// <summary>
    /// Dotted border.
    /// </summary>
    Dotted,
}

/// <summary>
/// Defines how an image should fit inside its PDF element bounds.
/// </summary>
public enum PdfImageFit
{
    /// <summary>
    /// Uses the renderer default image fit behavior.
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
    Scale,
}