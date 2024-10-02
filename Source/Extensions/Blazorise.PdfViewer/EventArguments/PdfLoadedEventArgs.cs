namespace Blazorise.PdfViewer;

/// <summary>
/// Provides data for the PDF loaded event, including the page number and total pages.
/// </summary>
public class PdfLoadedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PdfLoadedEventArgs"/> class
    /// with the specified page number and total pages.
    /// </summary>
    /// <param name="pageNumber">The current page number of the PDF document.</param>
    /// <param name="totalPages">The total number of pages in the PDF document.</param>
    public PdfLoadedEventArgs( int pageNumber, int totalPages )
    {
        PageNumber = pageNumber;
        TotalPages = totalPages;
    }

    /// <summary>
    /// Gets the current page number of the PDF document.
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// Gets the total number of pages in the PDF document.
    /// </summary>
    public int TotalPages { get; }
}