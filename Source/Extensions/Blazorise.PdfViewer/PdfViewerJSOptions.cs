using Blazorise.Modules;

namespace Blazorise.PdfViewer;

/// <summary>
/// Represents JavaScript options for initializing a PDF viewer component.
/// </summary>
public class PdfViewerJSOptions
{
    /// <summary>
    /// Gets or sets the source URL of the PDF document to be displayed.
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// Gets or sets the page number to display initially. Defaults to the first page.
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Gets or sets the scale factor for zooming the PDF document.
    /// </summary>
    public double Scale { get; set; }

    /// <summary>
    /// Gets or sets the rotation angle for the displayed page, in degrees.
    /// </summary>
    public double Rotation { get; set; }
}

/// <summary>
/// Represents JavaScript options for updating specific settings of a PDF viewer component dynamically.
/// </summary>
public class PdfViewerUpdateJSOptions
{
    /// <summary>
    /// Gets or sets the option for updating the source URL of the PDF document.
    /// </summary>
    public JSOptionChange<string> Source { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the displayed page number.
    /// </summary>
    public JSOptionChange<int> PageNumber { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the zoom scale of the PDF document.
    /// </summary>
    public JSOptionChange<double> Scale { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the rotation angle of the displayed page.
    /// </summary>
    public JSOptionChange<double> Rotation { get; set; }
}