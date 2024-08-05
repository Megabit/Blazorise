#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.PdfViewer;

/// <summary>
/// Represents a component for viewing PDF documents within the application.
/// </summary>
public partial class PdfViewer : BaseComponent, IAsyncDisposable
{
    #region Members

    private int totalPages;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        if ( Rendered )
        {
            var sourceChanged = parameters.TryGetValue<string>( nameof( Source ), out var paramSource ) && !Source.IsEqual( paramSource );
            var orientationChanged = parameters.TryGetValue<PdfOrientation>( nameof( Orientation ), out var paramOrientation ) && !Orientation.IsEqual( paramOrientation );

            if ( sourceChanged
                || orientationChanged )
            {
                ExecuteAfterRender( async () => await JSModule.UpdateOptions( ElementRef, ElementId, new
                {
                    source = new { Changed = sourceChanged, Value = paramSource },
                    rotation = new { Changed = orientationChanged, Value = paramOrientation.ToRotation() },
                } ) );
            }
        }

        await base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override Task OnInitializedAsync()
    {
        if ( JSModule == null )
        {
            DotNetObjectRef ??= DotNetObjectReference.Create( this );

            JSModule = new JSPdfViewerModule( JSRuntime, VersionProvider, BlazoriseOptions );
        }

        return base.OnInitializedAsync();
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            await JSModule.Initialize( DotNetObjectRef, ElementRef, ElementId, new
            {
                source = Source,
                scale = Scale,
                rotation = Orientation.ToRotation(),
            } );
        }

        await base.OnAfterRenderAsync( firstRender );
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSModule.SafeDestroy( ElementRef, ElementId );

            await JSModule.SafeDisposeAsync();

            if ( DotNetObjectRef != null )
            {
                DotNetObjectRef.Dispose();
                DotNetObjectRef = null;
            }
        }

        await base.DisposeAsync( disposing );
    }

    private async Task OnPreviousPageClicked()
    {
        await JSModule.PreviousPage( ElementRef, ElementId );
    }

    private async Task OnNextPageClicked()
    {
        await JSModule.NextPage( ElementRef, ElementId );
    }

    private async Task OnPageNumberChanged( int value )
    {
        if ( value < 1 )
            PageNumber = 1;
        else if ( value > totalPages )
            PageNumber = totalPages;
        else
            PageNumber = value;

        await JSModule.GoToPage( ElementRef, ElementId, PageNumber );
    }

    /// <summary>
    /// Notifies that a PDF document has been loaded.
    /// </summary>
    /// <param name="model">An instance of <see cref="PdfModel"/> containing the information about the loaded PDF document.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable( "NotifyDocumentLoaded" )]
    public async Task NotifyDocumentLoaded( PdfModel model )
    {
        if ( model is null )
            return;

        PageNumber = model.PageNumber;
        totalPages = model.TotalPages;

        await InvokeAsync( StateHasChanged );

        await Loaded.InvokeAsync( new PdfLoadedEventArgs( PageNumber, totalPages ) );
    }

    /// <summary>
    /// Notifies that the page number of the PDF document has changed.
    /// </summary>
    /// <param name="model">An instance of <see cref="PdfModel"/> containing the current page number of the PDF document.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable( "NotifyPageNumberChanged" )]
    public async Task NotifyPageNumberChanged( PdfModel model )
    {
        if ( model is null )
            return;

        PageNumber = model.PageNumber;
        totalPages = model.TotalPages;

        await PageNumberChanged.InvokeAsync( PageNumber );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Reference to the object that should be accessed through JSInterop.
    /// </summary>
    protected DotNetObjectReference<PdfViewer> DotNetObjectRef { get; private set; }

    /// <summary>
    /// Gets or sets the <see cref="JSPdfViewerModule"/> instance.
    /// </summary>
    protected JSPdfViewerModule JSModule { get; private set; }

    /// <summary>
    /// Gets or sets the <see cref="IJSRuntime"/>.
    /// </summary>
    [Inject] private IJSRuntime JSRuntime { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="IVersionProvider"/> for the JS module.
    /// </summary>
    [Inject] private IVersionProvider VersionProvider { get; set; }

    /// <summary>
    /// Gets or sets the blazorise options.
    /// </summary>
    [Inject] protected BlazoriseOptions BlazoriseOptions { get; set; }

    /// <summary>
    /// Gets or sets the source URL or path of the PDF document to be loaded.
    /// </summary>
    [Parameter] public string Source { get; set; }

    /// <summary>
    /// Gets or sets the current page number of the PDF document.
    /// The default value is <c>1</c>.
    /// </summary>
    [Parameter] public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the callback event that is triggered when the page number changes.
    /// </summary>
    [Parameter] public EventCallback<int> PageNumberChanged { get; set; }

    /// <summary>
    /// Gets or sets the callback event that is triggered when the PDF document is loaded.
    /// </summary>
    [Parameter] public EventCallback<PdfLoadedEventArgs> Loaded { get; set; }

    /// <summary>
    /// Gets or sets the scale factor for displaying the PDF document.
    /// The default value is <c>1</c>, which represents the original size.
    /// </summary>
    [Parameter] public double Scale { get; set; } = 1;

    /// <summary>
    /// Gets or sets the orientation of the PDF document.
    /// The default value is <see cref="PdfOrientation.Portrait"/>.
    /// </summary>
    [Parameter] public PdfOrientation Orientation { get; set; } = PdfOrientation.Portrait;

    /// <summary>
    /// Gets or sets the content to be rendered inside the component.
    /// </summary>
    /// <remarks>
    /// This property allows developers to define custom content within the PDF viewer component.
    /// </remarks>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}
