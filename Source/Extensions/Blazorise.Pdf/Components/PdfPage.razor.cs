#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Defines a PDF page inside a PDF document.
/// </summary>
public partial class PdfPage : ComponentBase
{
    #region Members

    private PdfPageContext pageContext;

    private PdfDocumentContext previousDocumentContext;

    private PdfPageDefinition previousDefinition;

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if ( DocumentContext is null )
            return;

        if ( previousDocumentContext is not null && previousDefinition is not null )
            previousDocumentContext.Definition.Pages.Remove( previousDefinition );

        PdfPageSize resolvedSize = Size == PdfPageSize.Custom && Width <= 0 && Height <= 0 ? DocumentContext.Definition.PageSize : Size;
        PdfOrientation resolvedOrientation = Orientation ?? DocumentContext.Definition.Orientation;
        double resolvedCustomWidth = Width > 0 ? Width : DocumentContext.Definition.PageWidth;
        double resolvedCustomHeight = Height > 0 ? Height : DocumentContext.Definition.PageHeight;
        (double width, double height) = PdfPageMetrics.Resolve( resolvedSize, resolvedOrientation, resolvedCustomWidth, resolvedCustomHeight );

        PdfPageDefinition definition = new()
        {
            Size = resolvedSize,
            Orientation = resolvedOrientation,
            Width = width,
            Height = height,
        };

        DocumentContext.Definition.Pages.Add( definition );

        pageContext = new( definition );
        previousDefinition = definition;
        previousDocumentContext = DocumentContext;
    }

    #endregion

    #region Parameters

    /// <summary>
    /// Provides the PDF document that receives this page definition.
    /// </summary>
    [CascadingParameter] protected PdfDocumentContext DocumentContext { get; set; }

    /// <summary>
    /// Page size for this page.
    /// </summary>
    [Parameter] public PdfPageSize Size { get; set; } = PdfPageSize.Custom;

    /// <summary>
    /// Page orientation for this page. If omitted, the document orientation is used.
    /// </summary>
    [Parameter] public PdfOrientation? Orientation { get; set; }

    /// <summary>
    /// Custom page width used when the page size is custom.
    /// </summary>
    [Parameter] public double Width { get; set; }

    /// <summary>
    /// Custom page height used when the page size is custom.
    /// </summary>
    [Parameter] public double Height { get; set; }

    /// <summary>
    /// PDF elements declared inside the page.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}