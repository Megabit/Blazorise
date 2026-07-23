namespace Blazorise.Pdf;

/// <summary>
/// Defines a rectangle in a PDF document.
/// </summary>
public class PdfRectangle : BasePdfElement
{
    #region Properties

    /// <inheritdoc />
    protected override PdfElementType ElementType => PdfElementType.Rectangle;

    #endregion
}