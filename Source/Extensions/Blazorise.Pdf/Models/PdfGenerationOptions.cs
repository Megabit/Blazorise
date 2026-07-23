namespace Blazorise.Pdf;

/// <summary>
/// Options used when generating a PDF document.
/// </summary>
public sealed class PdfGenerationOptions
{
    #region Properties

    /// <summary>
    /// Suggested output file name.
    /// </summary>
    public string FileName { get; set; } = "document.pdf";

    #endregion
}