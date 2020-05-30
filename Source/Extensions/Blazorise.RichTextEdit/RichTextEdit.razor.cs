#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.RichTextEdit
{
    public partial class RichTextEdit : BaseComponent
    {
        #region Members

        /// <summary>
        /// The disposables to cleanup.
        /// </summary>
        private readonly CompositeDisposable cleanup = new CompositeDisposable();

        /// <summary>
        /// The toolbar element reference.
        /// </summary>
        private ElementReference toolbarRef;

        /// <summary>
        /// The editor element reference.
        /// </summary>
        private ElementReference editorRef;

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
            base.Dispose( disposing );

            if ( initialized )
            {
                JSRuntime.InvokeVoidAsync( "blazoriseRichTextEdit.destroy", editorRef );
                cleanup.Dispose();
            }
        }

        /// <summary>
        /// Called when [first after render asynchronous].
        /// </summary>
        protected override async Task OnFirstAfterRenderAsync()
        {
            var dotNetRef = DotNetObjectReference
                .Create( this )
                .DisposeWith( cleanup );

            await JSRuntime.InvokeVoidAsync( "blazoriseRichTextEdit.initialize",
                dotNetRef,
                editorRef,
                Toolbar != null ? toolbarRef : default,
                ReadOnly,
                PlaceHolder,
                Theme == RichTextEditTheme.Snow ? "snow" : "bubble",
                nameof( OnContentChanged ),
                SubmitOnEnter,
                nameof( OnEnter ) );

            initialized = true;

            if ( Editor != null )
            {
                await OnContentChanged();
            }
        }

        void InitializationCheck()
        {
            if ( !initialized )
                throw new InvalidOperationException( "RichTextEdit not initialized yet" );
        }

        /// <summary>
        /// Sets the editor content as html asynchronous.
        /// </summary>
        public async ValueTask SetHtmlAsync( string html )
        {
            InitializationCheck();

            await JSRuntime.InvokeVoidAsync( "blazoriseRichTextEdit.setHtml", editorRef, html );
        }

        /// <summary>
        /// Gets the editor content as html asynchronous.
        /// </summary>
        public async ValueTask<string> GetHtmlAsync()
        {
            InitializationCheck();

            return await JSRuntime.InvokeAsync<string>( "blazoriseRichTextEdit.getHtml", editorRef );
        }

        /// <summary>
        /// Sets the editor content as Quill delta json asynchronous.
        /// </summary>
        /// <seealso href="https://quilljs.com/docs/delta/"/>
        public async ValueTask SetDeltaAsync( string deltaJson )
        {
            InitializationCheck();

            await JSRuntime.InvokeVoidAsync( "blazoriseRichTextEdit.setDelta", editorRef, deltaJson );
        }

        /// <summary>
        /// Gets the editor content as Quill delta asynchronous.
        /// </summary>
        /// <seealso href="https://quilljs.com/docs/delta/"/>
        public async ValueTask<string> GetDeltaAsync()
        {
            InitializationCheck();

            return await JSRuntime.InvokeAsync<string>( "blazoriseRichTextEdit.getDelta", editorRef );
        }

        /// <summary>
        /// Sets the editor plain text asynchronous.
        /// </summary>
        public async ValueTask SetTextAsync( string text )
        {
            InitializationCheck();

            await JSRuntime.InvokeVoidAsync( "blazoriseRichTextEdit.setText", editorRef, text );
        }

        /// <summary>
        /// Gets the editor plain text asynchronous.
        /// </summary>
        /// <seealso href="https://quilljs.com/docs/delta/"/>
        public async ValueTask<string> GetTextAsync()
        {
            InitializationCheck();

            return await JSRuntime.InvokeAsync<string>( "blazoriseRichTextEdit.getText", editorRef );
        }

        /// <summary>
        /// Clears the editor content asynchronous.
        /// </summary>
        public async ValueTask ClearAsync()
        {
            InitializationCheck();

            await JSRuntime.InvokeVoidAsync( "blazoriseRichTextEdit.clearContent", editorRef );

            await OnContentChanged();
        }

        /// <summary>
        /// Javascript callback for when content changes.
        /// </summary>
        [JSInvokable]
        public Task OnContentChanged()
            => ContentChanged.InvokeAsync( true );

        /// <summary>
        /// Javascript callback for when enter is pressed.
        /// </summary>
        [JSInvokable]
        public Task OnEnter()
            => EnterPressed.InvokeAsync( true );

        /// <summary>
        /// Toggles the readonly state
        /// </summary>
        private async void SetReadOnly( bool value )
        {
            if ( initialized )
            {
                await JSRuntime.InvokeVoidAsync( "blazoriseRichTextEdit.setReadOnly", editorRef, value );
            }
        }

        #endregion

        #region Properties

        [Inject] private IJSRuntime JSRuntime { get; set; }

        /// <summary>
        /// [Optional] Gets or sets the content of the toolbar.
        /// </summary>
        [Parameter] public RenderFragment Toolbar { get; set; }

        /// <summary>
        /// [Optional] Gets or sets the content visible in the editor.
        /// </summary>
        [Parameter] public RenderFragment Editor { get; set; }

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

                SetReadOnly( value );
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

        #endregion
    }
}
