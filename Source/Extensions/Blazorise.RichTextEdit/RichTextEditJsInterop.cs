#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.RichTextEdit
{
    internal sealed class RichTextEditJsInterop
    {
        #region Members

        private const string JsRoot = @"blazoriseRichTextEdit";

        private readonly IJSRuntime jsRuntime;
        private readonly RichTextEditOptions options;
        private bool isLoaded;
        private int loadStarted;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new RichTextEditJsInterop
        /// </summary>
        public RichTextEditJsInterop( IJSRuntime jsRuntime, RichTextEditOptions options )
        {
            this.jsRuntime = jsRuntime;
            this.options = options;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes given editor
        /// </summary>
        /// <returns>the cleanup routine</returns>
        public async ValueTask<IAsyncDisposable> InitializeEditor( RichTextEdit richTextEdit )
        {
            await InitializeJsInterop();

            var dotNetRef = DotNetObjectReference
                .Create( richTextEdit );

            await jsRuntime.InvokeVoidAsync( "blazoriseRichTextEdit.initialize",
                dotNetRef,
                richTextEdit.ElementRef,
                richTextEdit.ReadOnly,
                richTextEdit.PlaceHolder,
                richTextEdit.Theme == RichTextEditTheme.Snow ? "snow" : "bubble",
                richTextEdit.SubmitOnEnter,
                richTextEdit.ConfigureQuillJsMethod );

            return AsyncDisposable.Create( async () =>
            {
                var task = DestroyEditor( richTextEdit.EditorRef );

                try
                {
                    await task;
                }
                catch
                {
                    if ( !task.IsCanceled )
                    {
                        throw;
                    }
                }

                dotNetRef.Dispose();
            } );
        }

        private async ValueTask DestroyEditor( ElementReference editorRef )
        {
            await jsRuntime.InvokeVoidAsync( "blazoriseRichTextEdit.destroy", editorRef );
        }

        /// <summary>
        /// Sets the editor content as html asynchronous.
        /// </summary>
        public async ValueTask SetHtmlAsync( ElementReference editorRef, string html )
        {
            await jsRuntime.InvokeVoidAsync( $"{JsRoot}.setHtml", editorRef, html );
        }

        /// <summary>
        /// Gets the editor content as html asynchronous.
        /// </summary>
        public async ValueTask<string> GetHtmlAsync( ElementReference editorRef )
        {
            return await jsRuntime.InvokeAsync<string>( $"{JsRoot}.getHtml", editorRef );
        }

        /// <summary>
        /// Sets the editor content as Quill delta json asynchronous.
        /// </summary>
        /// <seealso href="https://quilljs.com/docs/delta/"/>
        public async ValueTask SetDeltaAsync( ElementReference editorRef, string deltaJson )
        {
            await jsRuntime.InvokeVoidAsync( $"{JsRoot}.setDelta", editorRef, deltaJson );
        }

        /// <summary>
        /// Gets the editor content as Quill delta asynchronous.
        /// </summary>
        /// <seealso href="https://quilljs.com/docs/delta/"/>
        public async ValueTask<string> GetDeltaAsync( ElementReference editorRef )
        {
            return await jsRuntime.InvokeAsync<string>( $"{JsRoot}.getDelta", editorRef );
        }

        /// <summary>
        /// Sets the editor plain text asynchronous.
        /// </summary>
        public async ValueTask SetTextAsync( ElementReference editorRef, string text )
        {
            await jsRuntime.InvokeVoidAsync( $"{JsRoot}.setText", editorRef, text );
        }

        /// <summary>
        /// Gets the editor plain text asynchronous.
        /// </summary>
        /// <seealso href="https://quilljs.com/docs/delta/"/>
        public async ValueTask<string> GetTextAsync( ElementReference editorRef )
        {
            return await jsRuntime.InvokeAsync<string>( $"{JsRoot}.getText", editorRef );
        }

        /// <summary>
        /// Clears the editor content asynchronous.
        /// </summary>
        public async ValueTask ClearAsync( ElementReference editorRef )
        {
            await jsRuntime.InvokeVoidAsync( $"{JsRoot}.clearContent", editorRef );
        }

        /// <summary>
        /// Sets the editor readonly state 
        /// </summary>
        public async ValueTask SetReadOnly( ElementReference editorRef, bool value )
        {
            await jsRuntime.InvokeVoidAsync( "blazoriseRichTextEdit.setReadOnly", editorRef, value );
        }

        private async ValueTask InitializeJsInterop()
        {
            if ( isLoaded || ( isLoaded = await IsLoaded() ) )
            {
                return;
            }

            await LoadDynamicReferences();
            await CheckIsLoaded();

            isLoaded = true;
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
            // Make sure both QuillJs and the blazorise javascript are loaded
            var quillLoaded = await jsRuntime.InvokeAsync<bool>( "eval", "(function(){ return typeof Quill !== 'undefined' })()" );
            return quillLoaded && await jsRuntime.InvokeAsync<bool>( "window.hasOwnProperty", JsRoot );
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

            await jsRuntime.InvokeVoidAsync( "eval", bootStrapScript.ToString() );
        }

        #endregion
    }
}