namespace Blazorise.Pdf;

/// <summary>
/// Builds PDF image definitions.
/// </summary>
public sealed class PdfImageBuilder : PdfElementBuilder
{
    #region Constructors

    /// <summary>
    /// Initializes a new PDF image builder.
    /// </summary>
    /// <param name="definition">The image definition.</param>
    public PdfImageBuilder( PdfElementDefinition definition )
        : base( definition )
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Sets the image source.
    /// </summary>
    /// <param name="source">The image source resolved by the configured <see cref="IPdfResourceResolver"/>.</param>
    /// <returns>The image builder.</returns>
    public PdfImageBuilder Source( string source )
    {
        Definition.Source = source;

        return this;
    }

    /// <summary>
    /// Defines how the image should fit inside the element bounds.
    /// </summary>
    /// <param name="fit">The image fit mode.</param>
    /// <returns>The image builder.</returns>
    public PdfImageBuilder Fit( PdfImageFit fit )
    {
        Definition.ImageFit = fit;

        return this;
    }

    #endregion
}