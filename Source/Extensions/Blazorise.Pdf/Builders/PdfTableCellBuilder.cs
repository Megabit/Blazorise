#region Using directives
using System;
using Blazorise;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Builds PDF table cell definitions.
/// </summary>
public sealed class PdfTableCellBuilder
{
    #region Members

    private readonly PdfTableCellDefinition definition;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new PDF table cell builder.
    /// </summary>
    /// <param name="definition">The cell definition.</param>
    public PdfTableCellBuilder( PdfTableCellDefinition definition )
    {
        this.definition = definition ?? throw new ArgumentNullException( nameof( definition ) );
    }

    #endregion

    #region Methods

    /// <summary>
    /// Adds text to the cell.
    /// </summary>
    /// <param name="text">The text value.</param>
    /// <returns>The text builder.</returns>
    public PdfTextBuilder Text( string text )
    {
        return new PdfTextBuilder( AddElement( PdfElementType.Text ) ).Text( text );
    }

    /// <summary>
    /// Adds an image to the cell.
    /// </summary>
    /// <param name="source">The image source resolved by the configured <see cref="IPdfResourceResolver"/>.</param>
    /// <returns>The image builder.</returns>
    public PdfImageBuilder Image( string source )
    {
        return new PdfImageBuilder( AddElement( PdfElementType.Image ) ).Source( source );
    }

    /// <summary>
    /// Adds a line to the cell.
    /// </summary>
    /// <param name="orientation">The line orientation.</param>
    /// <returns>The line builder.</returns>
    public PdfLineBuilder Line( Orientation orientation = Orientation.Horizontal )
    {
        return new PdfLineBuilder( AddElement( PdfElementType.Line ) ).Orientation( orientation );
    }

    /// <summary>
    /// Adds a rectangle to the cell.
    /// </summary>
    /// <returns>The rectangle builder.</returns>
    public PdfRectangleBuilder Rectangle()
    {
        return new( AddElement( PdfElementType.Rectangle ) );
    }

    /// <summary>
    /// Adds a table to the cell.
    /// </summary>
    /// <param name="configure">The table configuration.</param>
    /// <returns>The table builder.</returns>
    public PdfTableBuilder Table( Action<PdfTableBuilder> configure = null )
    {
        PdfTableBuilder builder = new( AddElement( PdfElementType.Table ) );
        configure?.Invoke( builder );

        return builder;
    }

    /// <summary>
    /// Adds an element to the cell.
    /// </summary>
    /// <param name="type">The element type.</param>
    /// <param name="configure">The element configuration.</param>
    /// <returns>The cell builder.</returns>
    public PdfTableCellBuilder Element( PdfElementType type, Action<PdfElementBuilder> configure )
    {
        if ( configure is null )
            throw new ArgumentNullException( nameof( configure ) );

        PdfElementDefinition element = AddElement( type );
        configure( new( element ) );

        return this;
    }

    private PdfElementDefinition AddElement( PdfElementType type )
    {
        PdfElementDefinition element = new()
        {
            Type = type,
            Width = definition.Width,
            Height = 24,
            Border = new()
            {
                Width = type is PdfElementType.Line or PdfElementType.Rectangle or PdfElementType.Table ? 1 : 0,
            },
        };

        definition.Elements.Add( element );

        return element;
    }

    #endregion
}