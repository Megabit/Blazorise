namespace Blazorise.Pdf;

/// <summary>
/// Defines the current stage of PDF generation.
/// </summary>
public enum PdfGenerationStage
{
    /// <summary>
    /// Font resources are being prepared.
    /// </summary>
    PreparingFonts,

    /// <summary>
    /// Document pages are being rendered.
    /// </summary>
    RenderingPages,

    /// <summary>
    /// The final PDF document is being written.
    /// </summary>
    WritingDocument,

    /// <summary>
    /// PDF generation has completed.
    /// </summary>
    Completed,
}