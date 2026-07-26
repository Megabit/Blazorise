#region Using directives
using System;
using Blazorise;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Builds PDF page definitions.
/// </summary>
public sealed class PdfPageBuilder
{
    #region Members

    private readonly PdfPageDefinition definition;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new PDF page builder.
    /// </summary>
    /// <param name="definition">The page definition.</param>
    public PdfPageBuilder( PdfPageDefinition definition )
    {
        this.definition = definition ?? throw new ArgumentNullException( nameof( definition ) );
    }

    #endregion

    #region Methods

    /// <summary>
    /// Adds text to the page.
    /// </summary>
    /// <param name="text">The text value.</param>
    /// <param name="x">The horizontal position.</param>
    /// <param name="y">The vertical position.</param>
    /// <param name="width">The text width.</param>
    /// <param name="height">The text height.</param>
    /// <returns>The element builder.</returns>
    public PdfElementBuilder Text( string text, double x, double y, double width, double height )
    {
        return AddElement( PdfElementType.Text, x, y, width, height ).Text( text );
    }

    /// <summary>
    /// Adds an image to the page.
    /// </summary>
    /// <param name="source">The image source.</param>
    /// <param name="x">The horizontal position.</param>
    /// <param name="y">The vertical position.</param>
    /// <param name="width">The image width.</param>
    /// <param name="height">The image height.</param>
    /// <returns>The element builder.</returns>
    public PdfElementBuilder Image( string source, double x, double y, double width, double height )
    {
        return AddElement( PdfElementType.Image, x, y, width, height ).Source( source );
    }

    /// <summary>
    /// Adds a line to the page.
    /// </summary>
    /// <param name="x">The horizontal position.</param>
    /// <param name="y">The vertical position.</param>
    /// <param name="width">The line width.</param>
    /// <param name="height">The line height.</param>
    /// <param name="orientation">The line orientation.</param>
    /// <returns>The element builder.</returns>
    public PdfElementBuilder Line( double x, double y, double width, double height, Orientation orientation = Orientation.Horizontal )
    {
        return AddElement( PdfElementType.Line, x, y, width, height ).Orientation( orientation );
    }

    /// <summary>
    /// Adds a rectangle to the page.
    /// </summary>
    /// <param name="x">The horizontal position.</param>
    /// <param name="y">The vertical position.</param>
    /// <param name="width">The rectangle width.</param>
    /// <param name="height">The rectangle height.</param>
    /// <returns>The element builder.</returns>
    public PdfElementBuilder Rectangle( double x, double y, double width, double height )
    {
        return AddElement( PdfElementType.Rectangle, x, y, width, height );
    }

    /// <summary>
    /// Adds a table to the page.
    /// </summary>
    /// <param name="x">The horizontal position.</param>
    /// <param name="y">The vertical position.</param>
    /// <param name="width">The table width.</param>
    /// <param name="height">The table height.</param>
    /// <param name="configure">The table configuration.</param>
    /// <returns>The table builder.</returns>
    public PdfTableBuilder Table( double x, double y, double width, double height, Action<PdfTableBuilder> configure = null )
    {
        PdfElementDefinition element = CreateElement( PdfElementType.Table, x, y, width, height );
        definition.Elements.Add( element );

        PdfTableBuilder builder = new( element );
        configure?.Invoke( builder );

        return builder;
    }

    private PdfElementBuilder AddElement( PdfElementType type, double x, double y, double width, double height )
    {
        PdfElementDefinition element = CreateElement( type, x, y, width, height );
        definition.Elements.Add( element );

        return new( element );
    }

    private static PdfElementDefinition CreateElement( PdfElementType type, double x, double y, double width, double height )
    {
        return new()
        {
            Type = type,
            X = x,
            Y = y,
            Width = width,
            Height = height,
        };
    }

    #endregion
}