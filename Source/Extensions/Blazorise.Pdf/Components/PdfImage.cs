namespace Blazorise.Pdf;

/// <summary>
/// Defines an image in a PDF document.
/// </summary>
public class PdfImage : BasePdfElement
{
    #region Properties

    /// <inheritdoc />
    protected override PdfElementType ElementType => PdfElementType.Image;

    #endregion
}