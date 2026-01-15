#region Using directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Markdown.Providers;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Markdown;

/// <summary>
/// Component for acts as a wrapper around the EasyMDE, a markdown editor.
/// </summary>
public partial class Markdown : BaseInputComponent<string, MarkdownClasses, MarkdownStyles>,
    IFileEntryOwner,
    IFileEntryNotifier,
    IAsyncDisposable
{
    #region Members

    private DotNetObjectReference<Markdown> dotNetObjectRef;

    private List<MarkdownToolbarButton> toolbarButtons;

    private bool hasPendingValue;

    private bool jsInitialized;

    private string pendingValue;

    private readonly ClassBuilder textAreaElementClassBuilder;

    private bool baseInputOptionsDirty;

    private bool baseInputOptionsUpdateScheduled;

    #endregion

    #region Methods

    public Markdown()
    {
        textAreaElementClassBuilder = new( BuildTextAreaElementClasses, builder => builder.Append( Classes?.TextArea ) );
    }

    protected override void OnInitialized()
    {
        JSModule ??= new JSMarkdownModule( JSRuntime, VersionProvider, BlazoriseOptions );

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override async Task OnAfterSetParametersAsync( ParameterView parameters )
    {
        await base.OnAfterSetParametersAsync( parameters );

        if ( Rendered && paramValue.Defined && paramValue.Changed )
        {
            var newValue = paramValue.Value ?? string.Empty;

            if ( jsInitialized )
            {
                ExecuteAfterRender( async () => await JSModule.SetValue( ElementId, newValue ) );
            }
            else
            {
                pendingValue = newValue;
                hasPendingValue = true;
            }
        }
    }

    /// <inheritdoc/>
    protected override async Task OnFirstAfterRenderAsync()
    {
        dotNetObjectRef ??= DotNetObjectReference.Create( this );

        await JSModule.Initialize( dotNetObjectRef, ElementRef, ElementId, new()
        {
            ReadOnly = ReadOnly,
            Disabled = Disabled,
            ClassNames = ClassNames,
            StyleNames = StyleNames,
            Attributes = GetEditorAttributes(),
            Value = Value ?? string.Empty,
            AutoDownloadFontAwesome = AutoDownloadFontAwesome,
            HideIcons = HideIcons,
            ShowIcons = ShowIcons,
            LineNumbers = LineNumbers,
            LineWrapping = LineWrapping,
            MinHeight = MinHeight,
            MaxHeight = MaxHeight,
            Placeholder = Placeholder,
            TabSize = TabSize,
            Theme = Theme,
            Direction = Direction,
            Toolbar = Toolbar != null && toolbarButtons?.Count > 0
                ? MarkdownActionProvider.Serialize( toolbarButtons )
                : null,
            ToolbarButtonClassPrefix = ToolbarButtonClassPrefix,
            ToolbarTips = ToolbarTips,
            UploadImage = UploadImage,
            ImageMaxSize = ImageMaxSize,
            ImageAccept = ImageAccept,
            ImageUploadEndpoint = ImageUploadEndpoint,
            ImagePathAbsolute = ImagePathAbsolute,
            ImageCSRFToken = ImageCSRFToken,
            ImageTexts = ImageTexts == null ? null : new MarkdownImageTextsOptions
            {
                SbInit = ImageTexts.Init,
                SbOnDragEnter = ImageTexts.OnDragEnter,
                SbOnDrop = ImageTexts.OnDrop,
                SbProgress = ImageTexts.Progress,
                SbOnUploaded = ImageTexts.OnUploaded,
                SizeUnits = ImageTexts.SizeUnits,
            },
            ErrorMessages = ErrorMessages,
            Autofocus = Autofocus,
            AutoRefresh = AutoRefresh,
            Autosave = Autosave,
            BlockStyles = BlockStyles,
            ForceSync = ForceSync,
            IndentWithTabs = IndentWithTabs,
            InputStyle = InputStyle,
            InsertTexts = InsertTexts,
            NativeSpellcheck = NativeSpellcheck,
            ParsingConfig = ParsingConfig,
            PreviewClass = PreviewClass,
            PreviewImagesInEditor = PreviewImagesInEditor,
            PromptTexts = PromptTexts,
            PromptURLs = PromptURLs,
            RenderingConfig = RenderingConfig,
            ScrollbarStyle = ScrollbarStyle,
            Shortcuts = Shortcuts,
            SideBySideFullscreen = SideBySideFullscreen,
            SpellChecker = SpellChecker,
            Status = Status,
            StyleSelectedText = StyleSelectedText,
            SyncSideBySidePreviewScroll = SyncSideBySidePreviewScroll,
            UnorderedListStyle = UnorderedListStyle,
            UsePreviewRender = PreviewRender != null
        } );

        jsInitialized = true;

        baseInputOptionsDirty = false;
        baseInputOptionsUpdateScheduled = false;

        await ApplyBaseInputOptions();

        if ( hasPendingValue )
        {
            hasPendingValue = false;

            await JSModule.SetValue( ElementId, pendingValue ?? string.Empty );

            pendingValue = null;
        }

        await base.OnFirstAfterRenderAsync();
    }

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        bool shouldUpdateBaseInputOptions = false;

        if ( Rendered )
        {
            if ( ( parameters.TryGetParameter( nameof( ReadOnly ), ReadOnly, out var readOnlyParameter ) && readOnlyParameter.Changed )
                || ( parameters.TryGetParameter( nameof( Disabled ), Disabled, out var disabledParameter ) && disabledParameter.Changed )
                || ( parameters.TryGetParameter( nameof( Class ), Class, out var classParameter ) && classParameter.Changed )
                || ( parameters.TryGetParameter( nameof( Classes ), Classes, newClasses => ReferenceEquals( newClasses, Classes ), out var classesParameter ) && classesParameter.Changed )
                || ( parameters.TryGetParameter( nameof( Styles ), Styles, newStyles => ReferenceEquals( newStyles, Styles ), out var stylesParameter ) && stylesParameter.Changed )
                || ( parameters.TryGetParameter( nameof( Style ), Style, out var styleParameter ) && styleParameter.Changed )
                || ( parameters.TryGetParameter( nameof( Attributes ), Attributes, newAttributes => ReferenceEquals( newAttributes, Attributes ), out var attributesParameter ) && attributesParameter.Changed ) )
            {
                shouldUpdateBaseInputOptions = true;
            }
        }

        await base.SetParametersAsync( parameters );

        if ( Rendered && shouldUpdateBaseInputOptions )
            RequestBaseInputOptionsUpdate();
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSModule.SafeDestroy( ElementRef, ElementId );

            await JSModule.SafeDisposeAsync();

            dotNetObjectRef?.Dispose();
            dotNetObjectRef = null;
        }

        await base.DisposeAsync( disposing );
    }

    /// <summary>
    /// Executes given action after the rendering is done.
    /// </summary>
    /// <remarks>Don't await this on the UI thread, because that will cause a deadlock.</remarks>
    protected async Task<T> ExecuteAfterRenderAsync<T>( Func<Task<T>> action, CancellationToken token = default )
    {
        var source = new TaskCompletionSource<T>();

        token.Register( () => source.TrySetCanceled() );

        ExecuteAfterRender( async () =>
        {
            try
            {
                var result = await action();
                source.TrySetResult( result );
            }
            catch ( TaskCanceledException )
            {
                source.TrySetCanceled();
            }
            catch ( Exception e )
            {
                source.TrySetException( e );
            }
        } );

        return await source.Task.ConfigureAwait( false );
    }

    /// <summary>
    /// Gets the markdown value.
    /// </summary>
    /// <returns>Markdown value.</returns>
    public async Task<string> GetValueAsync()
    {
        if ( Rendered )
            return await JSModule.GetValue( ElementId );

        return await ExecuteAfterRenderAsync( async () => await JSModule.GetValue( ElementId ) );
    }

    /// <summary>
    /// Sets the markdown value.
    /// </summary>
    /// <param name="value">Value to set.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SetValueAsync( string value )
    {
        value ??= string.Empty;

        if ( !value.IsEqual( Value ) )
            await CurrentValueHandler( value );

        if ( Rendered && jsInitialized )
        {
            await JSModule.SetValue( ElementId, value );
            return;
        }

        pendingValue = value;
        hasPendingValue = true;
    }

    /// <summary>
    /// Updates the internal markdown value. This method should only be called internally!
    /// </summary>
    /// <param name="value">New value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task UpdateInternalValue( string value )
    {
        value ??= string.Empty;

        if ( value.IsEqual( Value ) )
            return Task.CompletedTask;

        return InvokeAsync( () => CurrentValueHandler( value ) );
    }

    /// <summary>
    /// Adds the custom toolbar button.
    /// </summary>
    /// <param name="toolbarButton">Button instance.</param>
    internal protected void AddMarkdownToolbarButton( MarkdownToolbarButton toolbarButton )
    {
        toolbarButtons ??= [];
        toolbarButtons.Add( toolbarButton );
    }

    /// <summary>
    /// Removes the custom toolbar button.
    /// </summary>
    /// <param name="toolbarButton">Button instance.</param>
    internal protected void RemoveMarkdownToolbarButton( MarkdownToolbarButton toolbarButton )
    {
        toolbarButtons.Remove( toolbarButton );
    }

    /// <summary>
    /// Notifies the component that custom button has been clicked. This method is intended for internal framework use only and should not be called directly by user code.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [JSInvokable]
    public Task NotifyCustomButtonClicked( string name, object value )
    {
        return CustomButtonClicked.InvokeAsync( new MarkdownButtonEventArgs( name, value ) );
    }

    /// <summary>
    /// Notifies the component that file input value has changed. This method is intended for internal framework use only and should not be called directly by user code.
    /// </summary>
    /// <param name="fileEntries">Array of changed file</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public async Task NotifyImageUpload( FileEntry[] fileEntries )
    {
        foreach ( var file in fileEntries )
        {
            file.FileUploadEndedCallback = new();

            // So that method invocations on the file can be dispatched back here
            file.Owner = this;
        }

        if ( ImageUploadChanged is not null )
        {
            await ImageUploadChanged.Invoke( new( fileEntries ) );
        }

        foreach ( var file in fileEntries )
        {
            file.FileUploadEndedCallback.SetResult();
        }

        foreach ( var file in fileEntries )
        {
            await file.Owner.RemoveFileEntry( file );
        }

        await InvokeAsync( StateHasChanged );
    }

    /// <inheritdoc/>
    public Task UpdateFileStartedAsync( IFileEntry fileEntry )
    {
        // reset all
        ProgressProgress = 0;
        ProgressTotal = fileEntry.Size;
        Progress = 0;

        if ( ImageUploadStarted is not null )
            return ImageUploadStarted.Invoke( new( fileEntry ) );

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task UpdateFileEndedAsync( IFileEntry fileEntry, bool success, FileInvalidReason fileInvalidReason )
    {
        InvokeAsync( async () =>
        {
            if ( fileEntry.FileUploadEndedCallback is not null )
                await fileEntry.FileUploadEndedCallback.Task;

            if ( ImageUploadEnded is not null )
                await ImageUploadEnded.Invoke( new( fileEntry, success, fileInvalidReason ) );

            if ( !success )
            {
                await JSModule.NotifyImageUploadError( ElementId, fileEntry.ErrorMessage );
                return;
            }

            await JSModule.NotifyImageUploadSuccess( ElementId, fileEntry.UploadUrl ?? string.Empty );
        } );

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task UpdateFileWrittenAsync( IFileEntry fileEntry, long position, byte[] data )
    {
        if ( DisableProgressReport )
            return Task.CompletedTask;

        if ( ImageUploadWritten is not null )
            return ImageUploadWritten.Invoke( new( fileEntry, position, data ) );

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task UpdateFileProgressAsync( IFileEntry fileEntry, long progressProgress )
    {
        if ( DisableProgressReport )
            return Task.CompletedTask;

        ProgressProgress += progressProgress;

        var progress = Math.Round( (double)ProgressProgress / ProgressTotal, 3 );

        if ( Math.Abs( progress - Progress ) > double.Epsilon )
        {
            Progress = progress;

            if ( ImageUploadProgressed is not null )
                return ImageUploadProgressed.Invoke( new( fileEntry, Progress ) );
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task WriteToStreamAsync( IFileEntry fileEntry, Stream stream, CancellationToken cancellationToken = default )
    {
        return new RemoteFileEntryStreamReader( JSFileModule, ElementRef, fileEntry, this, MaxUploadImageChunkSize, ImageMaxSize )
            .WriteToStreamAsync( stream, cancellationToken );
    }

    /// <inheritdoc/>
    public Stream OpenReadStream( IFileEntry fileEntry, CancellationToken cancellationToken = default )
    {
        return new RemoteFileEntryStream( JSFileModule, ElementRef, fileEntry, this, ImageMaxSize, cancellationToken );
    }

    /// <inheritdoc/>
    public async Task RemoveFileEntry( IFileEntry fileEntry, CancellationToken cancellationToken = default )
    {
        await JSFileModule.RemoveFileEntry( ElementRef, fileEntry.Id );
    }

    /// <summary>
    /// Notifies the component that an error message has been received. This method is intended for internal framework use only and should not be called directly by user code.
    /// </summary>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    [JSInvokable]
    public Task NotifyErrorMessage( string errorMessage )
    {
        if ( ErrorCallback is not null )
            return ErrorCallback.Invoke( errorMessage );

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public override Task Focus( bool scrollToElement = true )
    {
        if ( jsInitialized )
            return JSModule.Focus( ElementId, scrollToElement ).AsTask();

        ExecuteAfterRender( () => JSModule.Focus( ElementId, scrollToElement ).AsTask() );

        return Task.CompletedTask;
    }

    /// <summary>
    /// Notifies the component that preview render has being requested. This method is intended for internal framework use only and should not be called directly by user code.
    /// </summary>
    /// <param name="plainText">Plain text of the markdown value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task<string> NotifyPreviewRender( string plainText )
    {
        if ( PreviewRender != null )
            return PreviewRender.Invoke( plainText );

        return Task.FromResult<string>( null );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        ValidationStatus validationStatus = ParentValidation?.Status ?? ValidationStatus.None;

        if ( validationStatus == ValidationStatus.Error )
        {
            builder.Append( "b-markdown-invalid" );
        }
        else if ( validationStatus == ValidationStatus.Success )
        {
            builder.Append( "b-markdown-valid" );
        }

        builder.Append( ClassProvider.MemoInputValidation( validationStatus ) );

        base.BuildClasses( builder );
    }

    protected virtual void BuildTextAreaElementClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.MemoInputValidation( ParentValidation?.Status ?? ValidationStatus.None ) );
    }

    internal protected override void DirtyClasses()
    {
        textAreaElementClassBuilder.Dirty();

        base.DirtyClasses();

        RequestBaseInputOptionsUpdate();
    }

    protected internal override void DirtyStyles()
    {
        base.DirtyStyles();

        RequestBaseInputOptionsUpdate();
    }

    private Dictionary<string, object> GetEditorAttributes()
    {
        Dictionary<string, object> editorAttributes = null;

        if ( Attributes is not null && Attributes.Count > 0 )
        {
            foreach ( KeyValuePair<string, object> attribute in Attributes )
            {
                if ( attribute.Key == null )
                    continue;

                if ( attribute.Key.Equals( "class", StringComparison.OrdinalIgnoreCase )
                    || attribute.Key.Equals( "style", StringComparison.OrdinalIgnoreCase )
                    || attribute.Key.Equals( "id", StringComparison.OrdinalIgnoreCase )
                    || attribute.Key.Equals( "readonly", StringComparison.OrdinalIgnoreCase )
                    || attribute.Key.Equals( "disabled", StringComparison.OrdinalIgnoreCase ) )
                {
                    continue;
                }

                editorAttributes ??= new Dictionary<string, object>( StringComparer.OrdinalIgnoreCase );
                editorAttributes[attribute.Key] = attribute.Value;
            }
        }

        if ( AriaInvalid is not null )
        {
            editorAttributes ??= new Dictionary<string, object>( StringComparer.OrdinalIgnoreCase );
            editorAttributes["aria-invalid"] = AriaInvalid;
        }

        if ( AriaDescribedBy is not null )
        {
            editorAttributes ??= new Dictionary<string, object>( StringComparer.OrdinalIgnoreCase );
            editorAttributes["aria-describedby"] = AriaDescribedBy;
        }

        return editorAttributes;
    }

    private void RequestBaseInputOptionsUpdate()
    {
        baseInputOptionsDirty = true;

        if ( baseInputOptionsUpdateScheduled )
            return;

        baseInputOptionsUpdateScheduled = true;

        ExecuteAfterRender( ApplyBaseInputOptionsIfDirty );
    }

    private async Task ApplyBaseInputOptionsIfDirty()
    {
        baseInputOptionsUpdateScheduled = false;

        if ( !baseInputOptionsDirty )
            return;

        if ( !jsInitialized )
            return;

        baseInputOptionsDirty = false;

        await ApplyBaseInputOptions();
    }

    private async Task ApplyBaseInputOptions()
    {
        if ( !jsInitialized )
            return;

        await JSModule.UpdateBaseInputOptions( ElementId, new MarkdownBaseInputJSOptions
        {
            ReadOnly = ReadOnly,
            Disabled = Disabled,
            ClassNames = ClassNames,
            StyleNames = StyleNames,
            Attributes = GetEditorAttributes()
        } );
    }

    /// <inheritdoc/>
    protected override Task<ParseValue<string>> ParseValueFromStringAsync( string value )
    {
        return Task.FromResult( new ParseValue<string>( true, value, null ) );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Number of processed bytes in current file.
    /// </summary>
    protected long ProgressProgress;

    /// <summary>
    /// Total number of bytes in currently processed file.
    /// </summary>
    protected long ProgressTotal;

    /// <summary>
    /// Percentage of the current file-read status.
    /// </summary>
    protected double Progress;

    /// <summary>
    /// Gets or sets the <see cref="JSMarkdownModule"/> instance.
    /// </summary>
    protected JSMarkdownModule JSModule { get; private set; }

    /// <summary>
    /// Gets the CSS class names applied to the underlying textarea element.
    /// </summary>
    protected string TextAreaElementClassNames => textAreaElementClassBuilder.Class;

    protected string TextAreaElementStyleNames => Styles?.TextArea;

    /// <summary>
    /// Gets or sets the <see cref="IJSFileModule"/> instance.
    /// </summary>
    [Inject] public IJSFileModule JSFileModule { get; set; }

    /// <summary>
    /// Gets or set the javascript runtime.
    /// </summary>
    [Inject] private IJSRuntime JSRuntime { get; set; }

    /// <summary>
    /// Gets or sets the version provider.
    /// </summary>
    [Inject] private IVersionProvider VersionProvider { get; set; }

    /// <summary>
    /// Gets or sets the blazorise options.
    /// </summary>
    [Inject] protected BlazoriseOptions BlazoriseOptions { get; set; }

    /// <summary>
    /// If set to true, force downloads Font Awesome (used for icons). If set to false, prevents downloading.
    /// </summary>
    [Parameter] public bool? AutoDownloadFontAwesome { get; set; }

    /// <summary>
    /// If set to true, enables line numbers in the editor.
    /// </summary>
    [Parameter] public bool LineNumbers { get; set; }

    /// <summary>
    /// If set to false, disable line wrapping. Defaults to true.
    /// </summary>
    [Parameter] public bool LineWrapping { get; set; } = true;

    /// <summary>
    /// Sets the minimum height for the composition area, before it starts auto-growing.
    /// Should be a string containing a valid CSS value like "500px". Defaults to "300px".
    /// </summary>
    [Parameter] public string MinHeight { get; set; } = "300px";

    /// <summary>
    /// Sets fixed height for the composition area. minHeight option will be ignored.
    /// Should be a string containing a valid CSS value like "500px". Defaults to undefined.
    /// </summary>
    [Parameter] public string MaxHeight { get; set; }

    /// <summary>
    /// If set, displays a custom placeholder message.
    /// </summary>
    [Parameter] public string Placeholder { get; set; }

    /// <summary>
    /// If set, customize the tab size. Defaults to 2.
    /// </summary>
    [Parameter] public int TabSize { get; set; } = 2;

    /// <summary>
    /// Override the theme. Defaults to easymde.
    /// </summary>
    [Parameter] public string Theme { get; set; } = "easymde";

    /// <summary>
    /// rtl or ltr. Changes text direction to support right-to-left languages. Defaults to ltr.
    /// </summary>
    [Parameter] public string Direction { get; set; } = "ltr";

    /// <summary>
    /// An array of icon names to hide. Can be used to hide specific icons shown by default without
    /// completely customizing the toolbar.
    /// </summary>
    [Parameter] public string[] HideIcons { get; set; } = ["side-by-side", "fullscreen"];

    /// <summary>
    /// An array of icon names to show. Can be used to show specific icons hidden by default without
    /// completely customizing the toolbar.
    /// </summary>
    [Parameter] public string[] ShowIcons { get; set; } = ["code", "table"];

    /// <summary>
    /// [Optional] Gets or sets the content of the toolbar.
    /// </summary>
    [Parameter] public RenderFragment Toolbar { get; set; }

    /// <summary>
    /// If set to false, disable toolbar button tips. Defaults to true.
    /// </summary>
    [Parameter] public bool ToolbarTips { get; set; } = true;

    /// <summary>
    /// Adds a prefix to the toolbar button classes when set. For example, a value of `"mde"` results in `"mde-bold"` for the Bold button.
    /// </summary>
    [Parameter] public string ToolbarButtonClassPrefix { get; set; } = "mde";

    /// <summary>
    /// Occurs after the custom toolbar button is clicked.
    /// </summary>
    [Parameter] public EventCallback<MarkdownButtonEventArgs> CustomButtonClicked { get; set; }

    /// <summary>
    /// If set to true, enables the image upload functionality, which can be triggered by drag-drop,
    /// copy-paste and through the browse-file window (opened when the user click on the upload-image icon).
    /// Defaults to true.
    /// </summary>
    [Parameter] public bool UploadImage { get; set; } = true;

    /// <summary>
    /// Gets or sets the max chunk size when uploading the file.
    /// Take note that if you're using <see cref="OpenReadStream(IFileEntry, CancellationToken)"/> you're provided with a stream and should configure the chunk size when handling with the stream.
    /// </summary>
    /// <remarks>
    /// https://docs.microsoft.com/en-us/aspnet/core/blazor/javascript-interoperability/call-dotnet-from-javascript?view=aspnetcore-6.0#stream-from-javascript-to-net
    /// </remarks>
    [Parameter] public int MaxUploadImageChunkSize { get; set; } = 20 * 1024;

    /// <summary>
    /// Gets or sets the Segment Fetch Timeout when uploading the file.
    /// </summary>
    [Parameter] public TimeSpan SegmentFetchTimeout { get; set; } = TimeSpan.FromMinutes( 1 );

    /// <summary>
    /// Maximum image size in bytes, checked before upload (note: never trust client, always check image
    /// size at server-side). Defaults to 1024*1024*2 (2Mb).
    /// </summary>
    [Parameter] public long ImageMaxSize { get; set; } = 1024 * 1024 * 2;

    /// <summary>
    /// A comma-separated list of mime-types used to check image type before upload (note: never trust client, always
    /// check file types at server-side). Defaults to image/png, image/jpeg.
    /// </summary>
    [Parameter] public string ImageAccept { get; set; } = "image/png, image/jpeg";

    /// <summary>
    /// The endpoint where the images data will be sent, via an asynchronous POST request. The server is supposed to
    /// save this image, and return a json response.
    /// </summary>
    [Parameter] public string ImageUploadEndpoint { get; set; }

    /// <summary>
    /// If set to true, will treat imageUrl from imageUploadFunction and filePath returned from imageUploadEndpoint as
    /// an absolute rather than relative path, i.e. not prepend window.location.origin to it.
    /// </summary>
    [Parameter] public string ImagePathAbsolute { get; set; }

    /// <summary>
    /// CSRF token to include with AJAX call to upload image. For instance used with Django backend.
    /// </summary>
    [Parameter] public string ImageCSRFToken { get; set; }

    /// <summary>
    /// Texts displayed to the user (mainly on the status bar) for the import image feature, where
    /// #image_name#, #image_size# and #image_max_size# will replaced by their respective values, that
    /// can be used for customization or internationalization.
    /// </summary>
    [Parameter] public MarkdownImageTexts ImageTexts { get; set; }

    /// <summary>
    /// Occurs every time the selected image has changed.
    /// </summary>
    [Parameter] public Func<FileChangedEventArgs, Task> ImageUploadChanged { get; set; }

    /// <summary>
    /// Occurs when an individual image upload has started.
    /// </summary>
    [Parameter] public Func<FileStartedEventArgs, Task> ImageUploadStarted { get; set; }

    /// <summary>
    /// Occurs when an individual image upload has ended.
    /// </summary>
    [Parameter] public Func<FileEndedEventArgs, Task> ImageUploadEnded { get; set; }

    /// <summary>
    /// Occurs every time the part of image has being written to the destination stream.
    /// </summary>
    [Parameter] public Func<FileWrittenEventArgs, Task> ImageUploadWritten { get; set; }

    /// <summary>
    /// Notifies the progress of image being written to the destination stream.
    /// </summary>
    [Parameter] public Func<FileProgressedEventArgs, Task> ImageUploadProgressed { get; set; }

    /// <summary>
    /// Errors displayed to the user, using the errorCallback option, where #image_name#, #image_size#
    /// and #image_max_size# will replaced by their respective values, that can be used for customization
    /// or internationalization.
    /// </summary>
    [Parameter] public MarkdownErrorMessages ErrorMessages { get; set; }

    /// <summary>
    /// A callback function used to define how to display an error message. Defaults to (errorMessage) => alert(errorMessage).
    /// </summary>
    [Parameter] public Func<string, Task> ErrorCallback { get; set; }

    /// <summary>
    /// Useful, when initializing the editor in a hidden DOM node. If set to { delay: 300 },
    /// it will check every 300 ms if the editor is visible and if positive, call CodeMirror's refresh().
    /// </summary>
    [Parameter] public MarkdownAutoRefresh AutoRefresh { get; set; }

    /// <summary>
    /// Saves the text that's being written and will load it back in the future.
    /// It will forget the text when the form it's contained in is submitted.
    /// </summary>
    [Parameter] public MarkdownAutosave Autosave { get; set; }

    /// <summary>
    /// Customize how certain buttons that style blocks of text behave.
    /// </summary>
    [Parameter] public MarkdownBlockStyles BlockStyles { get; set; }

    /// <summary>
    /// If set to true, force text changes made in EasyMDE to be immediately stored in original text area.
    /// Defaults to false.
    /// </summary>
    [Parameter] public bool ForceSync { get; set; }

    /// <summary>
    /// If set to false, indent using spaces instead of tabs. Defaults to true.
    /// </summary>
    [Parameter] public bool IndentWithTabs { get; set; } = true;

    /// <summary>
    /// textarea or contenteditable.
    /// Defaults to textarea for desktop and contenteditable for mobile.
    /// contenteditable option is necessary to enable nativeSpellcheck.
    /// </summary>
    [Parameter] public string InputStyle { get; set; }

    /// <summary>
    /// Customize how certain buttons that insert text behave. Takes an array with two elements.
    /// The first element will be the text inserted before the cursor or highlight, and the second
    /// element will be inserted after.
    /// For example, this is the default link value: ["[", "](http://)"].
    /// </summary>
    [Parameter] public MarkdownInsertTexts InsertTexts { get; set; }

    /// <summary>
    /// If set to false, disable native spell checker. Defaults to true.
    /// </summary>
    [Parameter] public bool NativeSpellcheck { get; set; } = true;

    /// <summary>
    /// Adjust settings for parsing the Markdown during editing (not previewing).
    /// </summary>
    [Parameter] public MarkdownParsingConfig ParsingConfig { get; set; }

    /// <summary>
    /// A space-separated strings that will be applied to the preview screen when activated.
    /// Defaults to "editor-preview".
    /// </summary>
    [Parameter] public string PreviewClass { get; set; } = "editor-preview";

    /// <summary>
    /// EasyMDE will show preview of images, false by default,
    /// preview for images will appear only for images on separate lines.
    /// </summary>
    [Parameter] public bool PreviewImagesInEditor { get; set; }

    /// <summary>
    /// Customize the text used to prompt for URLs.
    /// </summary>
    [Parameter] public MarkdownPromptTexts PromptTexts { get; set; }

    /// <summary>
    /// If set to true, a JS alert window appears asking for the link or image URL.
    /// Defaults to false.
    /// </summary>
    [Parameter] public bool PromptURLs { get; set; }

    /// <summary>
    /// Adjust settings for parsing the Markdown during previewing (not editing).
    /// </summary>
    [Parameter] public MarkdownRenderingConfig RenderingConfig { get; set; }

    /// <summary>
    /// Chooses a scrollbar implementation.
    /// The default is "native", showing native scrollbars.
    /// 
    /// The core library also provides the "null" style, which completely hides the scrollbars.
    /// Addons can implement additional scrollbar models.
    /// </summary>
    [Parameter] public string ScrollbarStyle { get; set; } = "native";

    /// <summary>
    /// Keyboard shortcuts associated with this instance.
    /// Defaults to the array of <see href="https://github.com/Ionaru/easy-markdown-editor#keyboard-shortcuts">shortcuts</see>.
    /// </summary>
    [Parameter] public MarkdownShortcuts Shortcuts { get; set; }

    /// <summary>
    /// If set to false, allows side-by-side editing without going into fullscreen. Defaults to false.
    /// </summary>
    [Parameter] public bool SideBySideFullscreen { get; set; }

    /// <summary>
    /// If set to false, disable the spell checker. Defaults to true
    /// </summary>
    [Parameter] public bool SpellChecker { get; set; } = true;

    /// <summary>
    /// If set to empty array, hide the status bar. Defaults to the array of built-in status bar items.
    /// Optionally, you can set an array of status bar items to include, and in what order.
    /// </summary>
    [Parameter] public string[] Status { get; set; }

    /// <summary>
    /// If set to false, remove the CodeMirror-selectedtext class from selected lines. Defaults to true.
    /// </summary>
    [Parameter] public bool StyleSelectedText { get; set; } = true;

    /// <summary>
    /// If set to false, disable syncing scroll in side by side mode. Defaults to true.
    /// </summary>
    [Parameter] public bool SyncSideBySidePreviewScroll { get; set; } = true;

    /// <summary>
    /// can be *, - or +. Defaults to *.
    /// </summary>
    [Parameter] public string UnorderedListStyle { get; set; } = "*";

    /// <summary>
    /// Gets or sets whether report progress should be disabled. By enabling this setting, ImageUploadProgressed and ImageUploadWritten callbacks won't be called. Internal file progress won't be tracked.
    /// <para>This setting can speed up file transfer considerably.</para>
    /// </summary>
    [Parameter] public bool DisableProgressReport { get; set; } = false;

    /// <summary>
    /// Custom function for parsing the plaintext Markdown and returning HTML. Used when user previews.
    /// </summary>
    [Parameter] public Func<string, Task<string>> PreviewRender { get; set; }

    /// <inheritdoc/>
    protected override string DefaultValue => string.Empty;

    #endregion
}