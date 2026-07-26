namespace Blazorise.Pdf;

/// <summary>
/// Defines text content in a PDF document.
/// </summary>
public class PdfText : BasePdfElement
{
    #region Properties

    /// <inheritdoc />
    protected override PdfElementType ElementType => PdfElementType.Text;

    #endregion
}