#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    private bool isLoaded;
    private int loadStarted;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new RichTextEditJsInterop
    /// </summary>
    public JSRichTextEditModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, RichTextEditOptions options )
        : base( jsRuntime, versionProvider )
    {
        this.options = options;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Initializes given editor
    /// </summary>
    /// <returns>the cleanup routine</returns>
    public async ValueTask<IAsyncDisposable> Initialize( RichTextEdit richTextEdit )
    {
        await InitializeJsInterop();

        if ( !isLoaded )
        {
            return AsyncDisposable.Create( () => ValueTask.CompletedTask );
        }

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

    private async ValueTask InitializeJsInterop()
    {
        try
        {
            if ( isLoaded || ( isLoaded = await IsLoaded() ) )
            {
                return;
            }

            await LoadDynamicReferences();
            await CheckIsLoaded();

            isLoaded = true;
        }
        catch
        {
            isLoaded = false;
        }
    }

    private async ValueTask LoadDynamicReferences()
    {
        // Make sure only one thread loads the javascript files
        if ( options.DynamicallyLoadReferences && Interlocked.Increment( ref loadStarted ) == 1 )
        {
            await LoadElementsAsync( options.DynamicReferences );
        }
    }

    private async ValueTask CheckIsLoaded()
    {
        var loaderLoopBreaker = 0;
        while ( !await IsLoaded() )
        {
            loaderLoopBreaker++;
            await Task.Delay( 100 );

            // Fail after 3s not to block and hide any other possible error
            if ( loaderLoopBreaker > 25 )
            {
                throw new InvalidOperationException( "Unable to initialize Blazorise RichTextEdit script" );
            }
        }
    }

    private async ValueTask<bool> IsLoaded()
    {
        // Make sure both QuillJs is loaded
        return await JSRuntime.InvokeAsync<bool>( "eval", "(function(){ return typeof Quill !== 'undefined' })()" );
    }

    /// <summary>
    /// Dynamically load an additional script or stylesheet.
    /// </summary>
    public async ValueTask LoadElementsAsync( IEnumerable<DynamicReference> references )
    {
        StringBuilder bootStrapScript = new( "(function() {" );

        foreach ( var (reference, index) in references.Select( ( reference, index ) => (reference, index) ) )
        {
            var element = $"e{index}";
            if ( reference.Type == DynamicReferenceType.Script )
            {
                bootStrapScript.AppendLine( $"    var {element} = document.createElement( 'script' ); " );
                bootStrapScript.AppendLine( $"    {element}.type = 'text/javascript';" );
                bootStrapScript.AppendLine( $"    {element}.src='{reference.Uri}'; " );
                bootStrapScript.AppendLine( $"    document['body'].appendChild( {element} );" );
            }
            else
            {
                bootStrapScript.AppendLine( $"    var {element} = document.createElement( 'link' ); " );
                bootStrapScript.AppendLine( $"    {element}.rel = 'stylesheet';" );
                bootStrapScript.AppendLine( $"    {element}.href='{reference.Uri}'; " );
                bootStrapScript.AppendLine( $"    document['head'].appendChild( {element} );" );
            }
        }

        bootStrapScript.AppendLine( "} )();" );

        await JSRuntime.InvokeVoidAsync( "eval", bootStrapScript.ToString() );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.RichTextEdit/richtextedit.js?v={VersionProvider.Version}";

    #endregion
}