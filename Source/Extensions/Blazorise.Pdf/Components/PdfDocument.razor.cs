#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Defines a PDF document by using declarative Razor syntax.
/// </summary>
public partial class PdfDocument : ComponentBase
{
    #region Members

    private PdfDocumentContext documentContext;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the generated document definition.
    /// </summary>
    public PdfDocumentDefinition Definition { get; private set; }

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        (double width, double height) = PdfPageMetrics.Resolve( PageSize, Orientation, PageWidth, PageHeight );

        Definition ??= new();
        Definition.Title = Title;
        Definition.PageSize = PageSize;
        Definition.Orientation = Orientation;
        Definition.PageWidth = width;
        Definition.PageHeight = height;

        documentContext ??= new( Definition );
    }

    #endregion

    #region Parameters

    /// <summary>
    /// Document title stored in the document definition.
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// Default page size used by pages that do not override it.
    /// </summary>
    [Parameter] public PdfPageSize PageSize { get; set; } = PdfPageSize.A4;

    /// <summary>
    /// Default page orientation used by pages that do not override it.
    /// </summary>
    [Parameter] public PdfOrientation Orientation { get; set; } = PdfOrientation.Portrait;

    /// <summary>
    /// Custom page width used when the page size is custom.
    /// </summary>
    [Parameter] public double PageWidth { get; set; } = PdfPageMetrics.A4Width;

    /// <summary>
    /// Custom page height used when the page size is custom.
    /// </summary>
    [Parameter] public double PageHeight { get; set; } = PdfPageMetrics.A4Height;

    /// <summary>
    /// PDF pages declared inside the document.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}