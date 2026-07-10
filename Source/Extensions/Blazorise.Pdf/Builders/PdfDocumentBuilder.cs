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