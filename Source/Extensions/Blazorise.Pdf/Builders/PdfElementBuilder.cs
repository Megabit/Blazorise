#region Using directives
using System;
using Blazorise;
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
    /// Sets the text value.
    /// </summary>
    /// <param name="text">The text value.</param>
    /// <returns>The element builder.</returns>
    public PdfElementBuilder Text( string text )
    {
        definition.Text = text;

        return this;
    }

    /// <summary>
    /// Enables or disables text wrapping inside the element bounds.
    /// </summary>
    /// <param name="wrap">A value indicating whether text should wrap.</param>
    /// <returns>The element builder.</returns>
    public PdfElementBuilder Wrap( bool wrap = true )
    {
        definition.Wrap = wrap;

        return this;
    }

    /// <summary>
    /// Sets the image source.
    /// </summary>
    /// <param name="source">The image source.</param>
    /// <returns>The element builder.</returns>
    public PdfElementBuilder Source( string source )
    {
        definition.Source = source;

        return this;
    }

    /// <summary>
    /// Sets the orientation used by line elements.
    /// </summary>
    /// <param name="orientation">The line orientation.</param>
    /// <returns>The element builder.</returns>
    public PdfElementBuilder Orientation( Orientation orientation )
    {
        definition.Orientation = orientation;

        return this;
    }

    /// <summary>
    /// Sets the font family. The built-in renderer maps the family to the closest PDF standard font (Helvetica, Times, or Courier).
    /// </summary>
    /// <param name="family">The font family.</param>
    /// <returns>The element builder.</returns>
    public PdfElementBuilder FontFamily( string family )
    {
        definition.Font.Family = family;

        return this;
    }

    /// <summary>
    /// Sets the font size.
    /// </summary>
    /// <param name="size">The font size.</param>
    /// <returns>The element builder.</returns>
    public PdfElementBuilder FontSize( double size )
    {
        definition.Font.Size = size;

        return this;
    }

    /// <summary>
    /// Sets the text color.
    /// </summary>
    /// <param name="color">The text color in hexadecimal format.</param>
    /// <returns>The element builder.</returns>
    public PdfElementBuilder TextColor( string color )
    {
        definition.Font.Color = color;

        return this;
    }

    /// <summary>
    /// Sets the text alignment.
    /// </summary>
    /// <param name="alignment">The text alignment.</param>
    /// <returns>The element builder.</returns>
    public PdfElementBuilder TextAlignment( TextAlignment alignment )
    {
        definition.Font.Alignment = alignment;

        return this;
    }

    /// <summary>
    /// Sets the text vertical alignment.
    /// </summary>
    /// <param name="alignment">The text vertical alignment.</param>
    /// <returns>The element builder.</returns>
    public PdfElementBuilder VerticalAlignment( VerticalAlignment alignment )
    {
        definition.Font.VerticalAlignment = alignment;

        return this;
    }

    /// <summary>
    /// Enables or disables bold text.
    /// </summary>
    /// <param name="bold">A value indicating whether text is bold.</param>
    /// <returns>The element builder.</returns>
    public PdfElementBuilder Bold( bool bold = true )
    {
        definition.Font.Bold = bold;

        return this;
    }

    /// <summary>
    /// Enables or disables italic text.
    /// </summary>
    /// <param name="italic">A value indicating whether text is italic.</param>
    /// <returns>The element builder.</returns>
    public PdfElementBuilder Italic( bool italic = true )
    {
        definition.Font.Italic = italic;

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
}