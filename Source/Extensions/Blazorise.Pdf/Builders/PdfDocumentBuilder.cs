#region Using directives
using System;
using Blazorise;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Builds PDF document definitions by using imperative fluent syntax.
/// </summary>
public sealed class PdfDocumentBuilder
{
    #region Members

    private readonly PdfDocumentDefinition definition = new();

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new PDF document builder.
    /// </summary>
    public PdfDocumentBuilder()
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates a new PDF document builder.
    /// </summary>
    /// <returns>The PDF document builder.</returns>
    public static PdfDocumentBuilder Create()
    {
        return new();
    }

    /// <summary>
    /// Sets the document title.
    /// </summary>
    /// <param name="title">The document title.</param>
    /// <returns>The PDF document builder.</returns>
    public PdfDocumentBuilder Title( string title )
    {
        definition.Title = title;

        return this;
    }

    /// <summary>
    /// Adds a document-scoped font family.
    /// </summary>
    /// <param name="font">Font family.</param>
    /// <returns>The PDF document builder.</returns>
    public PdfDocumentBuilder AddFont( FontFamily font )
    {
        definition.AddFont( font );

        return this;
    }

    /// <summary>
    /// Adds a document-scoped font family.
    /// </summary>
    /// <param name="name">Font family name.</param>
    /// <param name="regular">Regular font source.</param>
    /// <returns>The PDF document builder.</returns>
    public PdfDocumentBuilder AddFont( string name, FontSource regular )
    {
        return AddFont( name, regular, null, null, null );
    }

    /// <summary>
    /// Adds a document-scoped font family.
    /// </summary>
    /// <param name="name">Font family name.</param>
    /// <param name="regular">Regular font source.</param>
    /// <param name="bold">Bold font source.</param>
    /// <param name="italic">Italic font source.</param>
    /// <param name="boldItalic">Bold italic font source.</param>
    /// <returns>The PDF document builder.</returns>
    public PdfDocumentBuilder AddFont( string name, FontSource regular, FontSource bold = null, FontSource italic = null, FontSource boldItalic = null )
    {
        definition.AddFont( name, regular, bold, italic, boldItalic );

        return this;
    }

    /// <summary>
    /// Sets the default document page.
    /// </summary>
    /// <param name="size">The page size.</param>
    /// <param name="orientation">The page orientation.</param>
    /// <param name="width">The custom page width.</param>
    /// <param name="height">The custom page height.</param>
    /// <returns>The PDF document builder.</returns>
    public PdfDocumentBuilder PageSetup( PdfPageSize size, PdfOrientation orientation = PdfOrientation.Portrait, double width = 0, double height = 0 )
    {
        (double resolvedWidth, double resolvedHeight) = PdfPageMetrics.Resolve( size, orientation, width, height );

        definition.PageSize = size;
        definition.Orientation = orientation;
        definition.PageWidth = resolvedWidth;
        definition.PageHeight = resolvedHeight;

        return this;
    }

    /// <summary>
    /// Adds a page to the document.
    /// </summary>
    /// <param name="configure">The page configuration.</param>
    /// <returns>The PDF document builder.</returns>
    public PdfDocumentBuilder Page( Action<PdfPageBuilder> configure )
    {
        return Page( definition.PageSize, definition.Orientation, definition.PageWidth, definition.PageHeight, configure );
    }

    /// <summary>
    /// Adds a page to the document.
    /// </summary>
    /// <param name="size">The page size.</param>
    /// <param name="orientation">The page orientation.</param>
    /// <param name="configure">The page configuration.</param>
    /// <returns>The PDF document builder.</returns>
    public PdfDocumentBuilder Page( PdfPageSize size, PdfOrientation orientation, Action<PdfPageBuilder> configure )
    {
        return Page( size, orientation, 0, 0, configure );
    }

    /// <summary>
    /// Adds a page to the document.
    /// </summary>
    /// <param name="size">The page size.</param>
    /// <param name="orientation">The page orientation.</param>
    /// <param name="width">The custom page width.</param>
    /// <param name="height">The custom page height.</param>
    /// <param name="configure">The page configuration.</param>
    /// <returns>The PDF document builder.</returns>
    public PdfDocumentBuilder Page( PdfPageSize size, PdfOrientation orientation, double width, double height, Action<PdfPageBuilder> configure )
    {
        if ( configure is null )
            throw new ArgumentNullException( nameof( configure ) );

        (double resolvedWidth, double resolvedHeight) = PdfPageMetrics.Resolve( size, orientation, width, height );

        PdfPageDefinition page = new()
        {
            Size = size,
            Orientation = orientation,
            Width = resolvedWidth,
            Height = resolvedHeight,
        };

        definition.Pages.Add( page );
        configure( new( page ) );

        return this;
    }

    /// <summary>
    /// Builds the document definition.
    /// </summary>
    /// <returns>The document definition.</returns>
    public PdfDocumentDefinition Build()
    {
        return definition;
    }

    #endregion
}

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
    /// <returns>The element builder.</returns>
    public PdfElementBuilder Line( double x, double y, double width, double height )
    {
        return AddElement( PdfElementType.Line, x, y, width, height );
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
    public PdfElementBuilder TextAlignment( PdfTextAlignment alignment )
    {
        definition.Font.Alignment = alignment;

        return this;
    }

    /// <summary>
    /// Sets the text vertical alignment.
    /// </summary>
    /// <param name="alignment">The text vertical alignment.</param>
    /// <returns>The element builder.</returns>
    public PdfElementBuilder TextVerticalAlignment( PdfVerticalAlignment alignment )
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

/// <summary>
/// Builds PDF table definitions.
/// </summary>
public sealed class PdfTableBuilder : PdfElementBuilder
{
    #region Members

    private readonly PdfElementDefinition definition;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new PDF table builder.
    /// </summary>
    /// <param name="definition">The table definition.</param>
    public PdfTableBuilder( PdfElementDefinition definition )
        : base( definition )
    {
        this.definition = definition ?? throw new ArgumentNullException( nameof( definition ) );
    }

    #endregion

    #region Methods

    /// <summary>
    /// Adds a row to the table.
    /// </summary>
    /// <param name="height">The row height.</param>
    /// <param name="configure">The row configuration.</param>
    /// <returns>The table builder.</returns>
    public PdfTableBuilder Row( double height, Action<PdfTableRowBuilder> configure )
    {
        if ( configure is null )
            throw new ArgumentNullException( nameof( configure ) );

        PdfTableRowDefinition row = new()
        {
            Height = height,
        };

        definition.Rows.Add( row );
        configure( new( row ) );

        return this;
    }

    #endregion
}

/// <summary>
/// Builds PDF table row definitions.
/// </summary>
public sealed class PdfTableRowBuilder
{
    #region Members

    private readonly PdfTableRowDefinition definition;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new PDF table row builder.
    /// </summary>
    /// <param name="definition">The row definition.</param>
    public PdfTableRowBuilder( PdfTableRowDefinition definition )
    {
        this.definition = definition ?? throw new ArgumentNullException( nameof( definition ) );
    }

    #endregion

    #region Methods

    /// <summary>
    /// Adds a cell to the row.
    /// </summary>
    /// <param name="width">The cell width.</param>
    /// <param name="configure">The cell configuration.</param>
    /// <returns>The row builder.</returns>
    public PdfTableRowBuilder Cell( double width, Action<PdfTableCellBuilder> configure = null )
    {
        PdfTableCellDefinition cell = new()
        {
            Width = width,
        };

        definition.Cells.Add( cell );
        configure?.Invoke( new( cell ) );

        return this;
    }

    #endregion
}

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
    /// <returns>The element builder.</returns>
    public PdfElementBuilder Text( string text )
    {
        PdfElementDefinition element = new()
        {
            Type = PdfElementType.Text,
            Text = text,
            Width = definition.Width,
            Height = 24,
        };

        definition.Elements.Add( element );

        return new( element );
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

        PdfElementDefinition element = new()
        {
            Type = type,
            Width = definition.Width,
            Height = 24,
        };

        definition.Elements.Add( element );
        configure( new( element ) );

        return this;
    }

    #endregion
}