#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.RichTextEdit
{
    public partial class RichTextEdit : BaseRichTextEditComponent
    {
        #region Members

        /// <summary>
        /// The disposables to cleanup.
        /// </summary>
        private IDisposable cleanup;

        /// <summary>
        /// ReadOnly state.
        /// </summary>
        private bool readOnly;

        /// <summary>
        /// Is the editor initialized.
        /// </summary>
        private bool initialized;

        #endregion

        #region Methods

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && initialized && Rendered )
            {
                cleanup.Dispose();
            }

            base.Dispose( disposing );
        }

        /// <summary>
        /// Called when [first after render asynchronous].
        /// </summary>
        protected override async Task OnFirstAfterRenderAsync()
        {
            cleanup = await RteJsInterop.InitializeEditor( this );

            initialized = true;

            if ( Editor != null )
            {
                await OnContentChanged();
            }
        }

        /// <summary>
        /// Sets the editor content as html asynchronous.
        /// </summary>
        public async ValueTask SetHtmlAsync( string html )
        {
            if ( initialized )
            {
                await RteJsInterop.SetHtmlAsync( EditorRef, html );
            }
            else
            {
                ExecuteAfterRender( async () => await SetHtmlAsync( html ) );
            }
        }

        /// <summary>
        /// Gets the editor content as html asynchronous.
        /// </summary>
        public async ValueTask<string> GetHtmlAsync()
        {
            if ( initialized )
            {
                return await RteJsInterop.GetHtmlAsync( EditorRef );
            }

            return await ExecuteAfterRender( async () => await GetHtmlAsync() );
        }

        /// <summary>
        /// Sets the editor content as Quill delta json asynchronous.
        /// </summary>
        /// <seealso href="https://quilljs.com/docs/delta/"/>
        public async ValueTask SetDeltaAsync( string deltaJson )
        {
            if ( initialized )
            {
                await RteJsInterop.SetDeltaAsync( EditorRef, deltaJson );
            }
            else
            {
                ExecuteAfterRender( async () => await SetDeltaAsync( deltaJson ) );
            }
        }

        /// <summary>
        /// Gets the editor content as Quill delta asynchronous.
        /// </summary>
        /// <seealso href="https://quilljs.com/docs/delta/"/>
        public async ValueTask<string> GetDeltaAsync()
        {
            if ( initialized )
            {
                return await RteJsInterop.GetDeltaAsync( EditorRef );
            }

            return await ExecuteAfterRender( async () => await GetDeltaAsync() );
        }

        /// <summary>
        /// Sets the editor plain text asynchronous.
        /// </summary>
        public async ValueTask SetTextAsync( string text )
        {
            if ( initialized )
            {
                await RteJsInterop.SetTextAsync( EditorRef, text );
            }
            else
            {
                ExecuteAfterRender( async () => await SetTextAsync( text ) );
            }
        }

        /// <summary>
        /// Gets the editor plain text asynchronous.
        /// </summary>
        /// <seealso href="https://quilljs.com/docs/delta/"/>
        public async ValueTask<string> GetTextAsync()
        {
            if ( initialized )
            {
                return await RteJsInterop.GetTextAsync( EditorRef );
            }

            return await ExecuteAfterRender( async () => await GetTextAsync() );
        }

        /// <summary>
        /// Clears the editor content asynchronous.
        /// </summary>
        public async ValueTask ClearAsync()
        {
            if ( initialized )
            {

                await RteJsInterop.ClearAsync( EditorRef );
                await OnContentChanged();
            }
            else
            {
                ExecuteAfterRender( async () => await ClearAsync() );
            }
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
            if ( initialized )
            {
                await RteJsInterop.SetReadOnly( EditorRef, value );
            }
        }

        #endregion

        #region Properties

        [Inject] private RichTextEditJsInterop RteJsInterop { get; set; }

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
        /// Occurs when the content changes.
        /// </summary>
        [Parameter] public EventCallback ContentChanged { get; set; }

        /// <summary>
        /// Occurs when the enter key is pressed.
        /// </summary>
        /// <remarks>
        /// Only active when <see cref="SubmitOnEnter"/>
        /// </remarks>
        [Parameter] public EventCallback EnterPressed { get; set; }

        /// <summary>
        /// Occurs when the editor get focus.
        /// </summary>
        [Parameter] public EventCallback EditorFocus { get; set; }

        /// <summary>
        /// Occurs when the editor get focus.
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
}
