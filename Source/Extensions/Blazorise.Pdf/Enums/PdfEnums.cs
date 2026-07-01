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
/// Defines PDF measurement units used by public APIs.
/// </summary>
public enum PdfUnit
{
    /// <summary>
    /// PDF points.
    /// </summary>
    Point,

    /// <summary>
    /// Inches.
    /// </summary>
    Inch,

    /// <summary>
    /// Millimeters.
    /// </summary>
    Millimeter,

    /// <summary>
    /// Centimeters.
    /// </summary>
    Centimeter,
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
/// Defines PDF text alignment.
/// </summary>
public enum PdfTextAlignment
{
    /// <summary>
    /// Align text to the start of the element.
    /// </summary>
    Start,

    /// <summary>
    /// Center text inside the element.
    /// </summary>
    Center,

    /// <summary>
    /// Align text to the end of the element.
    /// </summary>
    End,
}

/// <summary>
/// Defines PDF text vertical alignment.
/// </summary>
public enum PdfVerticalAlignment
{
    /// <summary>
    /// Align text to the top of the element.
    /// </summary>
    Top,

    /// <summary>
    /// Center text vertically inside the element.
    /// </summary>
    Middle,

    /// <summary>
    /// Align text to the bottom of the element.
    /// </summary>
    Bottom,
}