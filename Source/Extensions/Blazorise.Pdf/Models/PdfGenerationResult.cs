namespace Blazorise.Pdf;

/// <summary>
/// Represents a generated PDF result.
/// </summary>
public sealed class PdfGenerationResult
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