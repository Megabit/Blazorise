#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Defines a PDF page inside a PDF document.
/// </summary>
public partial class PdfPage : ComponentBase, IDisposable
{
    #region Members

    private PdfPageContext pageContext;

    private readonly PdfPageDefinition definition = new();

    private PdfDocumentContext documentContext;

    #endregion

    #region Methods

    /// <inheritdoc />
    public override Task SetParametersAsync( ParameterView parameters )
    {
        bool definitionChanged = parameters.IsParameterChanged( Size )
            || parameters.IsParameterChanged( Orientation )
            || parameters.IsParameterChanged( Width )
            || parameters.IsParameterChanged( Height );

        Task task = base.SetParametersAsync( parameters );

        if ( definitionChanged )
            UpdateDefinition();

        return task;
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        pageContext = new( definition );
    }

    private void UpdateDefinition()
    {
        if ( documentContext is null )
            return;

        PdfPageSize resolvedSize = Size == PdfPageSize.Custom && Width <= 0 && Height <= 0 ? documentContext.Definition.PageSize : Size;
        PdfOrientation resolvedOrientation = Orientation ?? documentContext.Definition.Orientation;
        double resolvedCustomWidth = Width > 0 ? Width : documentContext.Definition.PageWidth;
        double resolvedCustomHeight = Height > 0 ? Height : documentContext.Definition.PageHeight;
        (double width, double height) = PdfPageMetrics.Resolve( resolvedSize, resolvedOrientation, resolvedCustomWidth, resolvedCustomHeight );

        definition.Size = resolvedSize;
        definition.Orientation = resolvedOrientation;
        definition.Width = width;
        definition.Height = height;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        documentContext?.Definition.Pages.Remove( definition );
        documentContext = null;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Provides the PDF document that receives this page definition.
    /// </summary>
    [CascadingParameter]
    protected PdfDocumentContext DocumentContext
    {
        get => documentContext;
        set
        {
            if ( ReferenceEquals( documentContext, value ) )
            {
                UpdateDefinition();
                return;
            }

            documentContext?.Definition.Pages.Remove( definition );
            documentContext = value;

            if ( documentContext is not null && !documentContext.Definition.Pages.Contains( definition ) )
                documentContext.Definition.Pages.Add( definition );

            UpdateDefinition();
        }
    }

    /// <summary>
    /// Page size for this page. Defaults to <c>A4</c>.
    /// </summary>
    /// <remarks>When Size is Custom and neither dimension is positive, uses the parent document's page size.</remarks>
    [Parameter] public PdfPageSize Size { get; set; } = PdfPageSize.Custom;

    /// <summary>
    /// Page orientation for this page. If omitted, the document orientation is used. Defaults to <c>Portrait</c>.
    /// </summary>
    /// <remarks>When omitted, uses the parent document's orientation.</remarks>
    [Parameter] public PdfOrientation? Orientation { get; set; }

    /// <summary>
    /// Custom page width used when the page size is custom. Defaults to <c>Document.PageWidth</c>.
    /// </summary>
    /// <remarks>When not positive, uses the parent document's custom page width. Named page sizes determine their own dimensions.</remarks>
    [Parameter] public double Width { get; set; }

    /// <summary>
    /// Custom page height used when the page size is custom. Defaults to <c>Document.PageHeight</c>.
    /// </summary>
    /// <remarks>When not positive, uses the parent document's custom page height. Named page sizes determine their own dimensions.</remarks>
    [Parameter] public double Height { get; set; }

    /// <summary>
    /// PDF elements declared inside the page.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}