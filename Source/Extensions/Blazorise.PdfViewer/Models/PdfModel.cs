namespace Blazorise.PdfViewer;

/// <summary>
/// Represents a model for PDF document metadata, including page number and total pages.
/// </summary>
public class PdfModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PdfModel"/> class with the specified page number and total pages.
    /// </summary>
    /// <param name="pageNumber">The current page number of the PDF document.</param>
    /// <param name="totalPages">The total number of pages in the PDF document.</param>
    /// <param name="scale">The scale of the PDF document.</param>
    public PdfModel( int pageNumber, int totalPages, double scale )
    {
        PageNumber = pageNumber;
        TotalPages = totalPages;
        Scale = scale;
    }

    /// <summary>
    /// Gets or sets the current page number of the PDF document.
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Gets or sets the total number of pages in the PDF document.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Gets or sets the scale of the PDF document.
    /// </summary>
    public double Scale { get; set; }
}