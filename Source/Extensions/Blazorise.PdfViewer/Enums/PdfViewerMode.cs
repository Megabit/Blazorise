namespace Blazorise.PdfViewer;

/// <summary>
/// Defines how PDF pages are displayed in the <see cref="PdfViewer"/>.
/// </summary>
public enum PdfViewerMode
{
    /// <summary>
    /// Displays one page at a time.
    /// </summary>
    SinglePage,

    /// <summary>
    /// Displays pages in a vertically scrollable continuous layout.
    /// </summary>
    Continuous,
}