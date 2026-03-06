namespace Blazorise.PdfViewer;

/// <summary>
/// Describes why a password was requested while loading a PDF document.
/// </summary>
public enum PdfPasswordRequestReason
{
    /// <summary>
    /// A password is required to open the document.
    /// </summary>
    Required = 1,

    /// <summary>
    /// The previously provided password was incorrect.
    /// </summary>
    Incorrect = 2,
}