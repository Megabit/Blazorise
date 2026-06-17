namespace Blazorise.Pdf;

/// <summary>
/// Options used when generating a PDF document.
/// </summary>
public sealed class PdfGenerateOptions
{
    #region Properties

    /// <summary>
    /// Suggested output file name.
    /// </summary>
    public string FileName { get; set; } = "document.pdf";

    #endregion
}

/// <summary>
/// Represents a generated PDF result.
/// </summary>
public sealed class PdfRenderResult
{
    #region Properties

    /// <summary>
    /// Generated PDF bytes.
    /// </summary>
    public byte[] Content { get; set; }

    /// <summary>
    /// Result content type.
    /// </summary>
    public string ContentType { get; set; } = "application/pdf";

    /// <summary>
    /// Suggested file name for the generated PDF.
    /// </summary>
    public string FileName { get; set; }

    #endregion
}