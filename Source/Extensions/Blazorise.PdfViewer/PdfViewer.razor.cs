#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Infrastructure;
using Blazorise.Localization;
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

    private readonly EventCallbackSubscriber nextPageSubscriber;
    private readonly EventCallbackSubscriber prevPageSubscriber;
    private readonly EventCallbackSubscriber<int> goToPageSubscriber;
    private readonly EventCallbackSubscriber<double> setScaleSubscriber;
    private readonly EventCallbackSubscriber printSubscriber;
    private readonly EventCallbackSubscriber downloadSubscriber;
    private ModalInstance passwordPromptModalInstance;
    private TaskCompletionSource<string> passwordPromptCompletionSource;
    private bool passwordPromptCanClose;
    private string pendingCanceledPasswordReloadSource;

    #endregion

    #region Constructors

    /// <summary>
    /// Default <see cref="PdfViewer"/> constructor.
    /// </summary>
    public PdfViewer()
    {
        nextPageSubscriber = new EventCallbackSubscriber( EventCallback.Factory.Create( this, NextPage ) );
        prevPageSubscriber = new EventCallbackSubscriber( EventCallback.Factory.Create( this, PreviousPage ) );
        goToPageSubscriber = new EventCallbackSubscriber<int>( EventCallback.Factory.Create<int>( this, GoToPage ) );
        setScaleSubscriber = new EventCallbackSubscriber<double>( EventCallback.Factory.Create<double>( this, SetScale ) );
        printSubscriber = new EventCallbackSubscriber( EventCallback.Factory.Create( this, Print ) );
        downloadSubscriber = new EventCallbackSubscriber( EventCallback.Factory.Create( this, Download ) );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnParametersSetAsync()
    {
        nextPageSubscriber.SubscribeOrReplace( ViewerState?.NextPageRequested );
        prevPageSubscriber.SubscribeOrReplace( ViewerState?.PrevPageRequested );
        goToPageSubscriber.SubscribeOrReplace( ViewerState?.GoToPageRequested );
        setScaleSubscriber.SubscribeOrReplace( ViewerState?.SetScaleRequested );
        printSubscriber.SubscribeOrReplace( ViewerState?.PrintRequested );
        downloadSubscriber.SubscribeOrReplace( ViewerState?.DownloadRequested );

        return base.OnParametersSetAsync();
    }

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        if ( Rendered )
        {
            var currentResolvedSource = ResolveSource( Source );
            var hasSourceParameter = parameters.TryGetValue<string>( nameof( Source ), out var paramSource );
            var parameterResolvedSource = ResolveSource( paramSource );
            var sourceChanged = hasSourceParameter && !currentResolvedSource.IsEqual( parameterResolvedSource );
            var pageNumberChanged = parameters.TryGetValue<int>( nameof( PageNumber ), out var paramPageNumber ) && !PageNumber.IsEqual( paramPageNumber );
            var scaleChanged = parameters.TryGetValue<double>( nameof( Scale ), out var paramScale ) && !Scale.IsEqual( paramScale );
            var orientationChanged = parameters.TryGetValue<PdfOrientation>( nameof( Orientation ), out var paramOrientation ) && !Orientation.IsEqual( paramOrientation );

            if ( !sourceChanged
                && !string.IsNullOrWhiteSpace( pendingCanceledPasswordReloadSource )
                && pendingCanceledPasswordReloadSource.IsEqual( parameterResolvedSource ) )
            {
                sourceChanged = true;
            }

            if ( sourceChanged )
            {
                pendingCanceledPasswordReloadSource = null;
            }

            if ( sourceChanged )
            {
                await ClosePasswordPrompt( completePendingRequest: true );
            }

            if ( sourceChanged
                || pageNumberChanged
                || scaleChanged
                || orientationChanged )
            {
                ExecuteAfterRender( async () =>
                {
                    if ( JSModule is not null )
                    {
                        await JSModule.UpdateOptions( ElementRef, ElementId, new()
                        {
                            Source = new( sourceChanged, parameterResolvedSource ),
                            PageNumber = new( pageNumberChanged, paramPageNumber ),
                            Scale = new( scaleChanged, paramScale ),
                            Rotation = new( orientationChanged, paramOrientation.ToRotation() )
                        } );
                    }
                } );
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
            if ( JSModule is not null )
            {
                await JSModule.Initialize( DotNetObjectRef, ElementRef, ElementId, new()
                {
                    Source = ResolveSource( Source ),
                    PageNumber = PageNumber,
                    Scale = Scale,
                    Rotation = Orientation.ToRotation()
                } );

            }
        }

        await base.OnAfterRenderAsync( firstRender );
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await ClosePasswordPrompt( completePendingRequest: true );

            nextPageSubscriber?.Dispose();
            prevPageSubscriber?.Dispose();
            goToPageSubscriber?.Dispose();
            setScaleSubscriber?.Dispose();
            printSubscriber?.Dispose();
            downloadSubscriber?.Dispose();

            if ( JSModule is not null )
            {
                await JSModule.SafeDestroy( ElementRef, ElementId );

                await JSModule.SafeDisposeAsync();
            }

            if ( DotNetObjectRef is not null )
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
        if ( JSModule is not null )
        {
            await JSModule.PreviousPage( ElementRef, ElementId );
        }
    }

    /// <summary>
    /// Navigates to the next page of the PDF document.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task NextPage()
    {
        if ( JSModule is not null )
        {
            await JSModule.NextPage( ElementRef, ElementId );
        }
    }

    /// <summary>
    /// Navigates to the specified page number in the PDF document.
    /// </summary>
    /// <param name="pageNumber">The page number to navigate to.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task GoToPage( int pageNumber )
    {
        if ( JSModule is not null )
        {
            await JSModule.GoToPage( ElementRef, ElementId, pageNumber );
        }
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
        if ( JSModule is not null )
        {
            await JSModule.SetScale( ElementRef, ElementId, scale );
        }
    }

    /// <summary>
    /// Prints the currently loaded PDF document.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Print()
    {
        if ( string.IsNullOrEmpty( Source ) )
            return;

        await PrintRequested.InvokeAsync();

        if ( JSModule is not null )
        {
            await JSModule.Print( ElementRef, ElementId, ResolveSource( Source ) );
        }
    }

    /// <summary>
    /// Downloads the currently loaded PDF document.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Download()
    {
        if ( string.IsNullOrEmpty( Source ) )
            return;

        await DownloadRequested.InvokeAsync();

        if ( JSModule is not null )
        {
            await JSModule.Download( ElementRef, ElementId, ResolveSource( Source ) );
        }
    }

    /// <summary>
    /// Requests the password needed to open a protected PDF document.
    /// </summary>
    /// <param name="model">An instance of <see cref="PdfPasswordRequestModel"/> containing password request details.</param>
    /// <returns>The entered password, or <c>null</c> if the request is canceled.</returns>
    [JSInvokable( "RequestPdfPassword" )]
    public Task<string> RequestPdfPassword( PdfPasswordRequestModel model )
        => RequestPdfPasswordInternal( model );

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

        await ClosePasswordPrompt();

        PageNumber = model.PageNumber;
        TotalPages = model.TotalPages;

        await InvokeAsync( StateHasChanged );

        await Loaded.InvokeAsync( new PdfLoadedEventArgs( PageNumber, TotalPages ) );

        if ( ViewerState?.PdfInitialized is not null )
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

        var pageNumberChanged = PageNumber != model.PageNumber;
        var scaleChanged = Scale != model.Scale;

        PageNumber = model.PageNumber;
        TotalPages = model.TotalPages;
        Scale = model.Scale;

        if ( pageNumberChanged )
        {
            await PageNumberChanged.InvokeAsync( PageNumber );
        }

        if ( scaleChanged )
        {
            await ScaleChanged.InvokeAsync( Scale );
        }

        if ( ViewerState?.PdfChanged is not null )
        {
            await ViewerState.PdfChanged.InvokeCallbackAsync( model );
        }
    }

    private async Task<string> RequestPdfPasswordInternal( PdfPasswordRequestModel model )
    {
        if ( model is null )
            return null;

        var reason = model.Reason == (int)PdfPasswordRequestReason.Incorrect
            ? PdfPasswordRequestReason.Incorrect
            : PdfPasswordRequestReason.Required;

        var args = new PdfPasswordRequestedEventArgs( reason, Math.Max( 1, model.Attempt ), model.Source ?? Source );

        var password = PasswordRequested is not null
            ? await PasswordRequested.Invoke( args )
            : await RequestPasswordFromDefaultPrompt( args );

        if ( password is null )
        {
            await PasswordCanceled.InvokeAsync();
            pendingCanceledPasswordReloadSource = ResolveSource( args.Source ?? Source );

            return null;
        }

        return password;
    }

    private async Task<string> RequestPasswordFromDefaultPrompt( PdfPasswordRequestedEventArgs args )
    {
        if ( !UseModalPasswordPrompt || ModalService?.ModalProvider is null )
            return null;

        var title = PasswordPromptOptions?.Title ?? Localizer["Password required"];
        var message = args.Reason == PdfPasswordRequestReason.Incorrect
            ? PasswordPromptOptions?.IncorrectPasswordMessage ?? Localizer["Incorrect password. Please try again."]
            : PasswordPromptOptions?.Message ?? Localizer["Enter the password to open this PDF."];
        var passwordPlaceholder = PasswordPromptOptions?.PasswordPlaceholder ?? Localizer["Password"];
        var confirmButtonText = PasswordPromptOptions?.ConfirmButtonText ?? Localizer["Open"];
        var cancelButtonText = PasswordPromptOptions?.CancelButtonText ?? Localizer["Cancel"];
        var requiredPasswordValidationMessage = PasswordPromptOptions?.RequiredPasswordValidationMessage ?? Localizer["Password is required."];

        passwordPromptCompletionSource = new TaskCompletionSource<string>( TaskCreationOptions.RunContinuationsAsynchronously );

        try
        {
            if ( passwordPromptModalInstance is null )
            {
                passwordPromptCanClose = false;

                passwordPromptModalInstance = await ModalService.Show<_PdfViewerPasswordPrompt>( title, parameters =>
                {
                    parameters.Add( x => x.Title, title );
                    parameters.Add( x => x.Message, message );
                    parameters.Add( x => x.PasswordPlaceholder, passwordPlaceholder );
                    parameters.Add( x => x.ConfirmButtonText, confirmButtonText );
                    parameters.Add( x => x.CancelButtonText, cancelButtonText );
                    parameters.Add( x => x.RequiredPasswordValidationMessage, requiredPasswordValidationMessage );
                    parameters.Add( x => x.SubmitRequested, EventCallback.Factory.Create<string>( this, OnPasswordPromptSubmitRequested ) );
                    parameters.Add( x => x.CancelRequested, EventCallback.Factory.Create( this, OnPasswordPromptCancelRequested ) );
                }, BuildPasswordPromptModalOptions( () => passwordPromptCanClose ) );
            }
            else
            {
                await ModalService.Show( passwordPromptModalInstance );
            }
        }
        catch
        {
            passwordPromptCompletionSource.TrySetResult( null );
        }

        return await passwordPromptCompletionSource.Task;
    }

    private Task OnPasswordPromptSubmitRequested( string password )
    {
        passwordPromptCompletionSource?.TrySetResult( password );

        return Task.CompletedTask;
    }

    private async Task OnPasswordPromptCancelRequested()
    {
        passwordPromptCompletionSource?.TrySetResult( null );

        await ClosePasswordPrompt();
    }

    private async Task ClosePasswordPrompt( bool completePendingRequest = false )
    {
        if ( completePendingRequest )
        {
            passwordPromptCompletionSource?.TrySetResult( null );
        }

        if ( passwordPromptModalInstance is not null )
        {
            passwordPromptCanClose = true;

            if ( ModalService?.ModalProvider is not null )
            {
                await ModalService.Hide( passwordPromptModalInstance );
            }

            passwordPromptModalInstance = null;
        }

        passwordPromptCanClose = false;
        passwordPromptCompletionSource = null;
    }

    private ModalInstanceOptions BuildPasswordPromptModalOptions( Func<bool> canClose )
    {
        var customModalOptions = PasswordPromptOptions?.ModalOptions;

        return new ModalInstanceOptions
        {
            UseModalStructure = false,
            Centered = customModalOptions?.Centered ?? true,
            Size = customModalOptions?.Size ?? ModalSize.Small,
            ShowBackdrop = customModalOptions?.ShowBackdrop,
            Animated = customModalOptions?.Animated,
            AnimationDuration = customModalOptions?.AnimationDuration,
            RenderMode = customModalOptions?.RenderMode,
            FocusTrap = customModalOptions?.FocusTrap,
            Attributes = customModalOptions?.Attributes,
            Background = customModalOptions?.Background ?? default,
            Border = customModalOptions?.Border,
            Casing = customModalOptions?.Casing ?? default,
            Class = customModalOptions?.Class,
            Clearfix = customModalOptions?.Clearfix ?? default,
            Display = customModalOptions?.Display,
            ElementId = customModalOptions?.ElementId,
            ElementRef = customModalOptions?.ElementRef ?? default,
            Flex = customModalOptions?.Flex,
            Float = customModalOptions?.Float ?? default,
            Height = customModalOptions?.Height,
            Margin = customModalOptions?.Margin,
            Overflow = customModalOptions?.Overflow,
            Padding = customModalOptions?.Padding,
            Position = customModalOptions?.Position,
            Scrollable = customModalOptions?.Scrollable,
            ScrollToTop = customModalOptions?.ScrollToTop,
            Shadow = customModalOptions?.Shadow ?? default,
            Stateful = customModalOptions?.Stateful,
            Style = customModalOptions?.Style,
            TextAlignment = customModalOptions?.TextAlignment ?? default,
            TextColor = customModalOptions?.TextColor ?? default,
            TextOverflow = customModalOptions?.TextOverflow ?? default,
            TextTransform = customModalOptions?.TextTransform ?? default,
            TextWeight = customModalOptions?.TextWeight ?? default,
            VerticalAlignment = customModalOptions?.VerticalAlignment ?? default,
            Visibility = customModalOptions?.Visibility ?? default,
            Width = customModalOptions?.Width,
            Opening = customModalOptions?.Opening,
            Opened = customModalOptions?.Opened,
            Closed = customModalOptions?.Closed,
            Closing = async e =>
            {
                if ( customModalOptions?.Closing is not null )
                {
                    await customModalOptions.Closing.Invoke( e );
                }

                if ( e.Cancel )
                    return;

                if ( !canClose() )
                {
                    e.Cancel = true;
                }
            },
        };
    }

    private string ResolveSource( string source )
    {
        if ( string.IsNullOrWhiteSpace( source ) )
            return source;

        if ( source.StartsWith( "_content/", StringComparison.OrdinalIgnoreCase )
             && Uri.TryCreate( NavigationManager?.Uri, UriKind.Absolute, out var currentUri ) )
        {
            var originUri = new Uri( $"{currentUri.Scheme}://{currentUri.Authority}/" );
            return new Uri( originUri, source ).ToString();
        }

        return source;
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
    /// Gets or sets the <see cref="NavigationManager"/>.
    /// </summary>
    [Inject] private NavigationManager NavigationManager { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="IVersionProvider"/> for the JS module.
    /// </summary>
    [Inject] private IVersionProvider VersionProvider { get; set; }

    /// <summary>
    /// Gets or sets the blazorise options.
    /// </summary>
    [Inject] protected BlazoriseOptions BlazoriseOptions { get; set; }

    /// <summary>
    /// Gets or sets the state of the PDF viewer.
    /// </summary>
    [CascadingParameter] public PdfViewerState ViewerState { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="IModalService"/> used for the default password prompt.
    /// </summary>
    [Inject] protected IModalService ModalService { get; set; }

    /// <summary>
    /// Gets or sets the DI registered <see cref="ITextLocalizer"/>.
    /// </summary>
    [Inject] protected ITextLocalizer<PdfViewer> Localizer { get; set; }

    /// <summary>
    /// Gets or sets the source URL or base64 formated string of the PDF document to be loaded.
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
    /// Gets or sets the callback event that is triggered when the PDF document is requested to be printed.
    /// </summary>
    [Parameter] public EventCallback PrintRequested { get; set; }

    /// <summary>
    /// Gets or sets the callback event that is triggered when the PDF document is requested to be downloaded.
    /// </summary>
    [Parameter] public EventCallback DownloadRequested { get; set; }

    /// <summary>
    /// Occurs when a password is required to open a protected PDF document.
    /// If not set, a default modal prompt is shown when <see cref="UseModalPasswordPrompt"/> is enabled.
    /// </summary>
    [Parameter] public Func<PdfPasswordRequestedEventArgs, Task<string>> PasswordRequested { get; set; }

    /// <summary>
    /// Gets or sets whether the default modal prompt should be used when <see cref="PasswordRequested"/> is not provided.
    /// </summary>
    [Parameter] public bool UseModalPasswordPrompt { get; set; } = true;

    /// <summary>
    /// Gets or sets options for customizing the default modal password prompt.
    /// </summary>
    [Parameter] public PdfPasswordPromptOptions PasswordPromptOptions { get; set; } = new();

    /// <summary>
    /// Gets or sets the callback event that is triggered when the password request is canceled.
    /// </summary>
    [Parameter] public EventCallback PasswordCanceled { get; set; }

    /// <summary>
    /// Gets or sets the scale factor for displaying the PDF document.
    /// The default value is <c>1</c>, which represents the original size.
    /// </summary>
    [Parameter] public double Scale { get; set; } = 1;

    /// <summary>
    /// Gets or sets the callback event that is triggered when the scale changes.
    /// </summary>
    [Parameter] public EventCallback<double> ScaleChanged { get; set; }

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