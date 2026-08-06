namespace Blazorise.Pdf;

/// <summary>
/// Builds PDF rectangle definitions.
/// </summary>
public sealed class PdfRectangleBuilder : PdfElementBuilder
{
    #region Constructors

    /// <summary>
    /// Initializes a new PDF rectangle builder.
    /// </summary>
    /// <param name="definition">The rectangle definition.</param>
    public PdfRectangleBuilder( PdfElementDefinition definition )
        : base( definition )
    {
    }

    #endregion
}