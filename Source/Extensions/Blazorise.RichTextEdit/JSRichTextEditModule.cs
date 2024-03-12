#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.RichTextEdit;

internal sealed class JSRichTextEditModule : BaseJSModule,
    IJSDestroyableModule
{
    #region Members

    private readonly RichTextEditOptions options;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new RichTextEditJsInterop
    /// </summary>
    public JSRichTextEditModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, RichTextEditOptions options )
        : base( jsRuntime, versionProvider )
    {
        this.options = options;
        ModuleLoaded = OnModuleLoaded;
    }

    #endregion

    #region Methods
    private async ValueTask OnModuleLoaded( IJSObjectReference @ref )
    {
        List<string> styles = new();
        if ( options.UseBubbleTheme )
            styles.Add( "bubble" );
        if ( options.UseBubbleTheme )
            styles.Add( "snow" );

        if ( styles.Count > 0 )
        {
            await @ref.InvokeVoidAsync( "loadStylesheets", styles, VersionProvider.Version );
        }
    }

    /// <summary>
    /// Initializes given editor
    /// </summary>
    /// <returns>the cleanup routine</returns>
    public async ValueTask<IAsyncDisposable> Initialize( RichTextEdit richTextEdit )
    {
        var dotNetRef = DotNetObjectReference.Create( richTextEdit );

        await InvokeSafeVoidAsync( "initialize",
            dotNetRef,
            richTextEdit.ElementRef,
            richTextEdit.ElementId,
            richTextEdit.ReadOnly,
            richTextEdit.PlaceHolder,
            richTextEdit.Theme == RichTextEditTheme.Snow ? "snow" : "bubble",
            richTextEdit.SubmitOnEnter,
            richTextEdit.ConfigureQuillJsMethod );

        return AsyncDisposable.Create( async () =>
        {
            await this.SafeDestroy( richTextEdit.EditorRef, richTextEdit.ElementId );

            dotNetRef.Dispose();
        } );
    }

    public ValueTask Destroy( ElementReference editorRef, string elementId )
        => InvokeSafeVoidAsync( "destroy", editorRef, elementId );

    /// <summary>
    /// Sets the editor content as html asynchronous.
    /// </summary>
    public ValueTask SetHtmlAsync( ElementReference editorRef, string html )
        => InvokeSafeVoidAsync( "setHtml", editorRef, html );

    /// <summary>
    /// Gets the editor content as html asynchronous.
    /// </summary>
    public ValueTask<string> GetHtmlAsync( ElementReference editorRef )
        => InvokeSafeAsync<string>( "getHtml", editorRef );

    /// <summary>
    /// Sets the editor content as Quill delta json asynchronous.
    /// </summary>
    /// <seealso href="https://quilljs.com/docs/delta/"/>
    public ValueTask SetDeltaAsync( ElementReference editorRef, string deltaJson )
        => InvokeSafeVoidAsync( "setDelta", editorRef, deltaJson );

    /// <summary>
    /// Gets the editor content as Quill delta asynchronous.
    /// </summary>
    /// <seealso href="https://quilljs.com/docs/delta/"/>
    public ValueTask<string> GetDeltaAsync( ElementReference editorRef )
        => InvokeSafeAsync<string>( "getDelta", editorRef );

    /// <summary>
    /// Sets the editor plain text asynchronous.
    /// </summary>
    public ValueTask SetTextAsync( ElementReference editorRef, string text )
        => InvokeSafeVoidAsync( "setText", editorRef, text );

    /// <summary>
    /// Gets the editor plain text asynchronous.
    /// </summary>
    /// <seealso href="https://quilljs.com/docs/delta/"/>
    public ValueTask<string> GetTextAsync( ElementReference editorRef )
        => InvokeSafeAsync<string>( "getText", editorRef );

    /// <summary>
    /// Clears the editor content asynchronous.
    /// </summary>
    public ValueTask ClearAsync( ElementReference editorRef )
        => InvokeSafeVoidAsync( "clearContent", editorRef );

    /// <summary>
    /// Sets the editor readonly state 
    /// </summary>
    public ValueTask SetReadOnly( ElementReference editorRef, bool value )
        => InvokeSafeVoidAsync( "setReadOnly", editorRef, value );

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.RichTextEdit/richtextedit.js?v={VersionProvider.Version}";

    #endregion
}