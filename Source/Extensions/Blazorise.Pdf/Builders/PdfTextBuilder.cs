#region Using directives
using Blazorise;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Builds PDF text definitions.
/// </summary>
public sealed class PdfTextBuilder : PdfElementBuilder
{
    #region Constructors

    /// <summary>
    /// Initializes a new PDF text builder.
    /// </summary>
    /// <param name="definition">The text definition.</param>
    public PdfTextBuilder( PdfElementDefinition definition )
        : base( definition )
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Sets the text value.
    /// </summary>
    /// <param name="text">The text value.</param>
    /// <returns>The text builder.</returns>
    public PdfTextBuilder Text( string text )
    {
        Definition.Text = text;

        return this;
    }

    /// <summary>
    /// Enables or disables text wrapping inside the element bounds.
    /// </summary>
    /// <param name="wrap">A value indicating whether text should wrap.</param>
    /// <returns>The text builder.</returns>
    public PdfTextBuilder Wrap( bool wrap = true )
    {
        Definition.Wrap = wrap;

        return this;
    }

    /// <summary>
    /// Sets the font family. The built-in renderer maps the family to the closest PDF standard font when no matching embedded font is available.
    /// </summary>
    /// <param name="family">The font family.</param>
    /// <returns>The text builder.</returns>
    public PdfTextBuilder FontFamily( string family )
    {
        Definition.Font.Family = family;

        return this;
    }

    /// <summary>
    /// Sets the font size.
    /// </summary>
    /// <param name="size">The font size.</param>
    /// <returns>The text builder.</returns>
    public PdfTextBuilder FontSize( double size )
    {
        Definition.Font.Size = size;

        return this;
    }

    /// <summary>
    /// Sets the text color.
    /// </summary>
    /// <param name="color">The text color in hexadecimal format.</param>
    /// <returns>The text builder.</returns>
    public PdfTextBuilder TextColor( string color )
    {
        Definition.Font.Color = color;

        return this;
    }

    /// <summary>
    /// Sets the text alignment.
    /// </summary>
    /// <param name="alignment">The text alignment.</param>
    /// <returns>The text builder.</returns>
    public PdfTextBuilder TextAlignment( TextAlignment alignment )
    {
        Definition.Font.Alignment = alignment;

        return this;
    }

    /// <summary>
    /// Sets the text vertical alignment.
    /// </summary>
    /// <param name="alignment">The text vertical alignment.</param>
    /// <returns>The text builder.</returns>
    public PdfTextBuilder VerticalAlignment( VerticalAlignment alignment )
    {
        Definition.Font.VerticalAlignment = alignment;

        return this;
    }

    /// <summary>
    /// Enables or disables bold text.
    /// </summary>
    /// <param name="bold">A value indicating whether text is bold.</param>
    /// <returns>The text builder.</returns>
    public PdfTextBuilder Bold( bool bold = true )
    {
        Definition.Font.Bold = bold;

        return this;
    }

    /// <summary>
    /// Enables or disables italic text.
    /// </summary>
    /// <param name="italic">A value indicating whether text is italic.</param>
    /// <returns>The text builder.</returns>
    public PdfTextBuilder Italic( bool italic = true )
    {
        Definition.Font.Italic = italic;

        return this;
    }

    #endregion
}