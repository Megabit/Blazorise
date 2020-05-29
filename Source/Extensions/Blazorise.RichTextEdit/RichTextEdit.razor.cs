using System.Threading.Tasks;
using Blazorise.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazorise.RichTextEdit
{
    public partial class RichTextEdit : BaseComponent
    {
        #region Members        
        /// <summary>The disposables to cleanup</summary>
        private readonly CompositeDisposable cleanup = new CompositeDisposable();
        
        /// <summary>The toolbar element reference</summary>
        private ElementReference toolbarRef;
        
        /// <summary>The editor element reference</summary>
        private ElementReference editorRef;
        
        /// <summary>ReadOnly state</summary>
        private bool readOnly;

        /// <summary>Is the editor initialized</summary>
        private bool initialized;
        #endregion

        #region Properties
        [Inject] private IJSRuntime JSRuntime { get; set; }

        /// <summary>[Optional] Gets or sets the content of the toolbar.</summary>
        [Parameter] public RenderFragment ToolbarContent { get; set; }

        /// <summary>[Optional] Gets or sets the content visible in the editor</summary>
        [Parameter] public RenderFragment EditorContent { get; set; }

        /// <summary>Gets or sets a value indicating whether the editor is ReadOnly.</summary>
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

        /// <summary>The theme (Snow or Bubble) of the editor</summary>
        [Parameter] public RichTextEditTheme Theme { get; set; } = RichTextEditTheme.Snow;

        /// <summary>Place holder text visible in empty editor.</summary>
        [Parameter] public string PlaceHolder { get; set; }

        /// <summary>Toolbar placed on the top or bottom of the editor</summary>
        [Parameter] public Placement ToolbarPosition { get; set; } = Placement.Top;

        /// <summary>Call <see cref="EnterPressed"/> event when user presses the ENTER key</summary>
        [Parameter] public bool SubmitOnEnter { get; set; }

        /// <summary>Occurs when the content changes</summary>
        [Parameter] public EventCallback<string> ContentChanged { get; set; }

        /// <summary>Occurs when the enter key is pressed.</summary>
        /// <remarks>Only active when <see cref="SubmitOnEnter"/></remarks>
        [Parameter] public EventCallback EnterPressed { get; set; }
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
                ToolbarContent != null ? toolbarRef : default,
                ReadOnly,
                PlaceHolder,
                Theme == RichTextEditTheme.Snow ? "snow" : "bubble",
                nameof( OnContentChanged ),
                SubmitOnEnter,
                nameof( OnEnter ) );

            initialized = true;

            if ( EditorContent != null )
            {
                var initialContent = await GetContentAsync();
                OnContentChanged( initialContent );
            }
        }

        /// <summary>
        /// Sets the editor content asynchronous.
        /// </summary>
        public ValueTask SetContentAsync( string html ) => JSRuntime.InvokeVoidAsync( "blazoriseRichTextEdit.setContent", editorRef, html );

        /// <summary>
        /// Gets the editor content asynchronous.
        /// </summary>
        public ValueTask<string> GetContentAsync() => JSRuntime.InvokeAsync<string>( "blazoriseRichTextEdit.getContent", editorRef );

        /// <summary>
        /// Clears the editor content asynchronous.
        /// </summary>
        public ValueTask ClearAsync() => JSRuntime.InvokeVoidAsync( "blazoriseRichTextEdit.clearContent", editorRef );

        /// <summary>
        /// Javascript callback for when content changes.
        /// </summary>
        [JSInvokable] public Task OnContentChanged( string html ) => ContentChanged.InvokeAsync( html );

        /// <summary>
        /// Javascript callback for when enter is pressed.
        /// </summary>
        [JSInvokable] public Task OnEnter() => EnterPressed.InvokeAsync( true );

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
    }
}
