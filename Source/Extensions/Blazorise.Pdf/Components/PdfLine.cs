namespace Blazorise.Pdf;

/// <summary>
/// Defines a line in a PDF document.
/// </summary>
public class PdfLine : BasePdfElement
{
    #region Properties

    /// <inheritdoc />
    protected override PdfElementType ElementType => PdfElementType.Line;

    #endregion
}