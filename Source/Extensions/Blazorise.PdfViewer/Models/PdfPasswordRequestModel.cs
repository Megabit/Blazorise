namespace Blazorise.PdfViewer;

/// <summary>
/// Represents a password request payload raised from JavaScript interop.
/// </summary>
public class PdfPasswordRequestModel
{
    /// <summary>
    /// Gets or sets the reason why the password is requested.
    /// </summary>
    public int Reason { get; set; }

    /// <summary>
    /// Gets or sets the number of attempts made so far.
    /// </summary>
    public int Attempt { get; set; }

    /// <summary>
    /// Gets or sets the source of the PDF currently being loaded.
    /// </summary>
    public string Source { get; set; }
}