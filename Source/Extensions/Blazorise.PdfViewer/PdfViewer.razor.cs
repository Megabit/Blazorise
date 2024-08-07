#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Infrastructure;
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

    private readonly EventCallbackSubscriber<object> nextPageSubscriber;
    private readonly EventCallbackSubscriber<object> prevPageSubscriber;
    private readonly EventCallbackSubscriber<int> goToPageSubscriber;
    private readonly EventCallbackSubscriber<double> setScaleSubscriber;

    #endregion

    #region Constructors

    /// <summary>
    /// Default <see cref="PdfViewer"/> constructor.
    /// </summary>
    public PdfViewer()
    {
        nextPageSubscriber = new EventCallbackSubscriber<object>( EventCallback.Factory.Create<object>( this, NextPage ) );
        prevPageSubscriber = new EventCallbackSubscriber<object>( EventCallback.Factory.Create<object>( this, PreviousPage ) );
        goToPageSubscriber = new EventCallbackSubscriber<int>( EventCallback.Factory.Create<int>( this, GoToPage ) );
        setScaleSubscriber = new EventCallbackSubscriber<double>( EventCallback.Factory.Create<double>( this, SetScale ) );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnParametersSetAsync()
    {
        nextPageSubscriber.SubscribeOrMove( ViewerState?.NextPageRequested );
        prevPageSubscriber.SubscribeOrMove( ViewerState?.PrevPageRequested );
        goToPageSubscriber.SubscribeOrMove( ViewerState?.GoToPageRequested );
        setScaleSubscriber.SubscribeOrMove( ViewerState?.SetScaleRequested );

        return base.OnParametersSetAsync();
    }

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
            nextPageSubscriber.Dispose();
            prevPageSubscriber.Dispose();
            goToPageSubscriber.Dispose();
            setScaleSubscriber.Dispose();

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

    /// <summary>
    /// Navigates to the previous page of the PDF document.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task PreviousPage()
    {
        await JSModule.PreviousPage( ElementRef, ElementId );
    }

    /// <summary>
    /// Navigates to the next page of the PDF document.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task NextPage()
    {
        await JSModule.NextPage( ElementRef, ElementId );
    }

    /// <summary>
    /// Navigates to the specified page number in the PDF document.
    /// </summary>
    /// <param name="pageNumber">The page number to navigate to.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task GoToPage( int pageNumber )
    {
        await JSModule.GoToPage( ElementRef, ElementId, pageNumber );
    }

    /// <summary>
    /// Sets the scale factor for displaying the PDF document.
    /// </summary>
    /// <param name="scale">
    /// The scale factor to set. A value of <c>1.0</c> represents 100% (original size).
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SetScale( double scale )
    {
        await JSModule.SetScale( ElementRef, ElementId, scale );
    }

    /// <summary>
    /// Notifies that a PDF document has been loaded.
    /// </summary>
    /// <param name="model">An instance of <see cref="PdfModel"/> containing the information about the loaded PDF document.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable( "NotifyPdfInitialized" )]
    public async Task NotifyPdfInitialized( PdfModel model )
    {
        if ( model is null )
            return;

        PageNumber = model.PageNumber;
        TotalPages = model.TotalPages;

        await InvokeAsync( StateHasChanged );

        await Loaded.InvokeAsync( new PdfLoadedEventArgs( PageNumber, TotalPages ) );

        if ( ViewerState != null )
        {
            await ViewerState.PdfInitialized.InvokeCallbackAsync( model );
        }
    }

    /// <summary>
    /// Notifies that the page number of the PDF document has changed.
    /// </summary>
    /// <param name="model">An instance of <see cref="PdfModel"/> containing the current page number of the PDF document.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable( "NotifyPdfChanged" )]
    public async Task NotifyPdfChanged( PdfModel model )
    {
        if ( model is null )
            return;

        PageNumber = model.PageNumber;
        TotalPages = model.TotalPages;

        await PageNumberChanged.InvokeAsync( PageNumber );

        if ( ViewerState != null )
        {
            await ViewerState.PdfChanged.InvokeCallbackAsync( model );
        }
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
    internal protected JSPdfViewerModule JSModule { get; private set; }

    /// <summary>
    /// Gets the total number of pages in the PDF document.
    /// </summary>
    /// <value>
    /// The total number of pages available in the currently loaded PDF document.
    /// </value>
    public int TotalPages { get; private set; }

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
    /// Gets or sets the state of the PDF viewer.
    /// </summary>
    [Parameter] public PdfViewerState ViewerState { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the component.
    /// </summary>
    /// <remarks>
    /// This property allows developers to define custom content within the PDF viewer component.
    /// </remarks>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}
