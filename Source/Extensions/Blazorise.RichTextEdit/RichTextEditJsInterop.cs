#region Using directives
using System;
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
                nameof( RichTextEdit.OnContentChanged ),
                richTextEdit.SubmitOnEnter,
                nameof( RichTextEdit.OnEnter ),
                nameof( RichTextEdit.OnEditorFocus ),
                nameof( RichTextEdit.OnEditorBlur ),
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

            await LoadScript();

            isLoaded = true;
        }

        private async ValueTask LoadScript()
        {
            // Make sure only one thread loads the javascript files
            if ( options.DynamicLoadReferences && Interlocked.Increment( ref loadStarted ) == 1 )
            {
                var qjsVersion = options.QuillJsVersion;

                await LoadElementAsync( $@"https://cdn.quilljs.com/{qjsVersion}/quill.js", true );
                await LoadElementAsync( @"_content/Blazorise.RichTextEdit/blazorise.richtextedit.js", true );
                await LoadElementAsync( @"_content/Blazorise.RichTextEdit/Blazorise.RichTextEdit.bundle.scp.css", false );

                if ( options.UseBubbleTheme )
                {
                    await LoadElementAsync( $@"https://cdn.quilljs.com/{qjsVersion}/quill.bubble.css", false );
                }

                if ( options.UseShowTheme )
                {
                    await LoadElementAsync( $@"https://cdn.quilljs.com/{qjsVersion}/quill.snow.css", false );
                }
            }

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

        private async ValueTask LoadElementAsync( string uri, bool isScript )
        {
            string bootStrapScript;
            if ( isScript )
            {
                bootStrapScript = "(function()" +
                                  "{" +
                                  "var s = document.createElement('script'); " +
                                  "s.type = 'text/javascript';" +
                                  $"s.src='{uri}'; " +
                                  "document['body'].appendChild(s); " +
                                  "})();";
            }
            else
            {
                bootStrapScript = "(function()" +
                                  "{" +
                                  "var l = document.createElement('link'); " +
                                  "l.rel = 'stylesheet';" +
                                  $"l.href='{uri}'; " +
                                  "document['head'].appendChild(l); " +
                                  "})();";
            }

            await jsRuntime.InvokeVoidAsync( "eval", bootStrapScript );
        }

        #endregion
    }
}