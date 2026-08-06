#region Using directives
using System;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Builds PDF element definitions.
/// </summary>
public class PdfElementBuilder
{
    #region Members

    private readonly PdfElementDefinition definition;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new PDF element builder.
    /// </summary>
    /// <param name="definition">The element definition.</param>
    public PdfElementBuilder( PdfElementDefinition definition )
    {
        this.definition = definition ?? throw new ArgumentNullException( nameof( definition ) );
    }

    #endregion

    #region Methods

    /// <summary>
    /// Controls whether content is clipped to the element bounds.
    /// </summary>
    /// <param name="clipContent">A value indicating whether content should be clipped.</param>
    /// <returns>The element builder.</returns>
    public PdfElementBuilder ClipContent( bool clipContent = true )
    {
        definition.ClipContent = clipContent;

        return this;
    }

    /// <summary>
    /// Sets the border color.
    /// </summary>
    /// <param name="color">The border color in hexadecimal format.</param>
    /// <returns>The element builder.</returns>
    public PdfElementBuilder BorderColor( string color )
    {
        definition.Border.Color = color;

        return this;
    }

    /// <summary>
    /// Sets the border width.
    /// </summary>
    /// <param name="width">The border width.</param>
    /// <returns>The element builder.</returns>
    public PdfElementBuilder BorderWidth( double width )
    {
        definition.Border.Width = width;

        return this;
    }

    /// <summary>
    /// Sets the border style.
    /// </summary>
    /// <param name="style">The border style.</param>
    /// <returns>The element builder.</returns>
    public PdfElementBuilder BorderStyle( PdfBorderStyle style )
    {
        definition.Border.Style = style;

        return this;
    }

    /// <summary>
    /// Sets the background color.
    /// </summary>
    /// <param name="color">The background color in hexadecimal format.</param>
    /// <returns>The element builder.</returns>
    public PdfElementBuilder BackgroundColor( string color )
    {
        definition.Appearance.BackgroundColor = color;

        return this;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the element definition.
    /// </summary>
    protected PdfElementDefinition Definition => definition;

    #endregion
}