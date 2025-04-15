#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.RichTextEdit;

public partial class RichTextEdit : BaseRichTextEditComponent, IAsyncDisposable
{
    #region Members

    /// <summary>
    /// The disposables to cleanup.
    /// </summary>
    private IAsyncDisposable cleanup;

    /// <summary>
    /// ReadOnly state.
    /// </summary>
    private bool readOnly;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await cleanup.SafeDisposeAsync();
        }

        await base.DisposeAsync( disposing );
    }

    /// <summary>
    /// Called when [first after render asynchronous].
    /// </summary>
    protected override async Task OnFirstAfterRenderAsync()
    {
        cleanup = await JSModule.Initialize( this );

        if ( Editor != null )
        {
            await OnContentChanged();
        }

        await base.OnFirstAfterRenderAsync();
    }

    /// <summary>
    /// Sets the editor content as HTML asynchronously.
    /// </summary>
    /// <remarks>
    /// Improper handling of HTML can lead to cross-site scripting (XSS). Ensure that the HTML content is properly sanitized before setting it in the editor.
    /// </remarks>
    public async ValueTask SetHtmlAsync( string html )
    {
        if ( Rendered )
        {
            await JSModule.SetHtmlAsync( EditorRef, html );
            return;
        }

        await InvokeAsync( () => ExecuteAfterRender( async () => await JSModule.SetHtmlAsync( EditorRef, html ) ) );
    }

    /// <summary>
    /// Gets the editor content as HTML asynchronously.
    /// </summary>
    /// <param name="htmlOptions">Options to control how the HTML is retrieved, such as semantic HTML, or innerHTML.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous operation, containing the HTML content of the editor.</returns>
    public async ValueTask<string> GetHtmlAsync( RichTextEditHtmlOptions htmlOptions = null )
    {
        htmlOptions ??= RichTextEditHtmlOptions.SemanticHtml();

        if ( Rendered )
        {
            return await JSModule.GetHtmlAsync( EditorRef, htmlOptions );
        }

        return await ExecuteAfterRender( async () => await JSModule.GetHtmlAsync( EditorRef, htmlOptions ) );
    }

    /// <summary>
    /// Sets the editor content as a Quill Delta JSON asynchronously.
    /// </summary>
    /// <param name="deltaJson">The Delta JSON string representing the editor's content.</param>
    /// <remarks>
    /// The Quill Delta format is a rich text format used by the Quill editor. See the official Quill.js documentation for more details.
    /// </remarks>
    /// <seealso href="https://quilljs.com/docs/delta/"/>
    public async ValueTask SetDeltaAsync( string deltaJson )
    {
        if ( Rendered )
        {
            await JSModule.SetDeltaAsync( EditorRef, deltaJson );
            return;
        }

        await InvokeAsync( () => ExecuteAfterRender( async () => await JSModule.SetDeltaAsync( EditorRef, deltaJson ) ) );
    }

    /// <summary>
    /// Gets the editor content as a Quill Delta JSON asynchronously.
    /// </summary>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous operation, containing the Delta JSON string of the editor's content.</returns>
    /// <seealso href="https://quilljs.com/docs/delta/"/>
    public async ValueTask<string> GetDeltaAsync()
    {
        if ( Rendered )
        {
            return await JSModule.GetDeltaAsync( EditorRef );
        }

        return await ExecuteAfterRender( async () => await JSModule.GetDeltaAsync( EditorRef ) );
    }

    /// <summary>
    /// Sets the editor content as plain text asynchronously.
    /// </summary>
    /// <param name="text">The plain text to set in the editor.</param>
    public async ValueTask SetTextAsync( string text )
    {
        if ( Rendered )
        {
            await JSModule.SetTextAsync( EditorRef, text );

            return;
        }

        await InvokeAsync( () => ExecuteAfterRender( async () => await JSModule.SetTextAsync( EditorRef, text ) ) );
    }

    /// <summary>
    /// Gets the editor content as plain text asynchronously.
    /// </summary>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous operation, containing the plain text of the editor's content.</returns>
    public async ValueTask<string> GetTextAsync()
    {
        if ( Rendered )
        {
            return await JSModule.GetTextAsync( EditorRef );
        }

        return await ExecuteAfterRender( async () => await JSModule.GetTextAsync( EditorRef ) );
    }

    /// <summary>
    /// Clears the editor content asynchronously.
    /// </summary>
    public async ValueTask ClearAsync()
    {
        await InvokeAsync( () => ExecuteAfterRender( async () =>
        {
            await JSModule.ClearAsync( EditorRef );

            await OnContentChanged();
        } ) );
    }

    /// <summary>
    /// Javascript callback for when content changes.
    /// </summary>
    [JSInvokable]
    public Task OnContentChanged() => ContentChanged.InvokeAsync( true );

    /// <summary>
    /// Javascript callback for when enter is pressed.
    /// </summary>
    [JSInvokable]
    public Task OnEnter() => EnterPressed.InvokeAsync( true );

    /// <summary>
    /// Javascript callback for when editor get focus.
    /// </summary>
    [JSInvokable]
    public Task OnEditorFocus() => EditorFocus.InvokeAsync( true );

    /// <summary>
    /// Javascript callback for when editor lost focus.
    /// </summary>
    [JSInvokable]
    public Task OnEditorBlur() => EditorBlur.InvokeAsync( true );

    /// <summary>
    /// Toggles the readonly state
    /// </summary>
    private async Task SetReadOnly( bool value )
    {
        await InvokeAsync( () => ExecuteAfterRender( async () => await JSModule.SetReadOnly( EditorRef, value ) ) );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the <see cref="JSRichTextEditModule"/> instance.
    /// </summary>
    [Inject] private JSRichTextEditModule JSModule { get; set; }

    /// <summary>
    /// [Optional] Gets or sets the content of the toolbar.
    /// </summary>
    [Parameter] public RenderFragment Toolbar { get; set; }

    /// <summary>
    /// [Optional] Gets or sets the content visible in the editor.
    /// </summary>
    [Parameter]
    public RenderFragment Editor { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the editor is ReadOnly.
    /// </summary>
    [Parameter]
    public bool ReadOnly
    {
        get => readOnly;
        set
        {
            readOnly = value;

            ExecuteAfterRender( async () =>
            {
                await SetReadOnly( value );
            } );
        }
    }

    /// <summary>
    /// The theme (Snow or Bubble) of the editor.
    /// </summary>
    [Parameter] public RichTextEditTheme Theme { get; set; } = RichTextEditTheme.Snow;

    /// <summary>
    /// Place holder text visible in empty editor.
    /// </summary>
    [Parameter] public string PlaceHolder { get; set; }

    /// <summary>
    /// Toolbar placed on the top or bottom of the editor.
    /// </summary>
    [Parameter] public Placement ToolbarPosition { get; set; } = Placement.Top;

    /// <summary>
    /// Call <see cref="EnterPressed"/> event when user presses the ENTER key.
    /// </summary>
    [Parameter] public bool SubmitOnEnter { get; set; } = false;

    /// <summary>
    /// Occurs when the content within the editor changes.
    /// </summary>
    [Parameter] public EventCallback ContentChanged { get; set; }

    /// <summary>
    /// Occurs when the enter key is pressed within the editor.
    /// </summary>
    /// <remarks>
    /// This event is triggered only when <see cref="SubmitOnEnter"/> is enabled.
    /// </remarks>
    [Parameter] public EventCallback EnterPressed { get; set; }

    /// <summary>
    /// Occurs when the editor gains focus.
    /// </summary>
    [Parameter] public EventCallback EditorFocus { get; set; }

    /// <summary>
    /// Occurs when the editor loses focus.
    /// </summary>
    [Parameter] public EventCallback EditorBlur { get; set; }

    /// <summary>
    /// The toolbar element reference.
    /// </summary>
    public ElementReference ToolbarRef { get; protected set; }

    /// <summary>
    /// The editor element reference.
    /// </summary>
    public ElementReference EditorRef { get; protected set; }

    /// <summary>
    /// [Optional] The javascript method to call to configure additional QuillJs modules and or add custom bindings.
    /// </summary>
    /// <example>
    /// ConfigureQuillJsMethod = "myNamespace.configureQuillJs"
    ///
    /// JS:
    /// window.myNamespace {
    ///    configureQuillJs: (options) => {
    ///        Quill.register('modules/blotFormatter', QuillBlotFormatter.default);
    ///        options.debug = "log";
    ///        options.modules.blotFormatter = { };
    ///        return options;
    ///    }
    /// };
    /// </example>
    /// <seealso href="https://github.com/quilljs/awesome-quill"/>
    [Parameter] public string ConfigureQuillJsMethod { get; set; }

    #endregion
}