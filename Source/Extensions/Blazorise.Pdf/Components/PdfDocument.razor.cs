#region Using directives
using System.Threading.Tasks;
using Blazorise.Extensions;
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

    #region Methods

    /// <inheritdoc />
    public override Task SetParametersAsync( ParameterView parameters )
    {
        bool definitionChanged = parameters.IsParameterChanged( Title )
            || parameters.IsParameterChanged( PageSize )
            || parameters.IsParameterChanged( Orientation )
            || parameters.IsParameterChanged( PageWidth )
            || parameters.IsParameterChanged( PageHeight );

        Task task = base.SetParametersAsync( parameters );

        if ( definitionChanged )
            UpdateDefinition();

        return task;
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        documentContext = new( Definition );
    }

    private void UpdateDefinition()
    {
        (double width, double height) = PdfPageMetrics.Resolve( PageSize, Orientation, PageWidth, PageHeight );

        Definition.Title = Title;
        Definition.PageSize = PageSize;
        Definition.Orientation = Orientation;
        Definition.PageWidth = width;
        Definition.PageHeight = height;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the generated document definition.
    /// </summary>
    public PdfDocumentDefinition Definition { get; } = new();

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