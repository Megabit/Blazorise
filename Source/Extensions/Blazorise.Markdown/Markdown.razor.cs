#region Using directives
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Markdown
{
    /// <summary>
    /// Component for acts as a wrapper around the EasyMDE, a markdown editor.
    /// </summary>
    public partial class Markdown : BaseComponent
    {
        #region Members

        private DotNetObjectReference<Markdown> dotNetObjectRef;

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            if ( JSModule == null )
            {
                JSModule = new JSMarkdownModule( JSRuntime, VersionProvider );
            }

            base.OnInitialized();
        }

        /// <inheritdoc/>
        public override async Task SetParametersAsync( ParameterView parameters )
        {
            if ( Initialized && parameters.TryGetValue<string>( nameof( Value ), out var newValue ) && newValue != Value )
            {
                ExecuteAfterRender( () => SetValueAsync( newValue ) );
            }

            await base.SetParametersAsync( parameters );
        }

        /// <inheritdoc/>
        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            await base.OnAfterRenderAsync( firstRender );

            if ( firstRender )
            {
                dotNetObjectRef ??= DotNetObjectReference.Create( this );

                await JSModule.Initialize( dotNetObjectRef, ElementRef, ElementId, new
                {
                    Value,
                    AutoDownloadFontAwesome,
                    HideIcons,
                    ShowIcons,
                    LineNumbers,
                    LineWrapping,
                    MinHeight,
                    MaxHeight,
                    Placeholder,
                    TabSize,
                    Theme,
                    Direction,
                    Toolbar,
                    ToolbarTips,
                } );

                Initialized = true;
            }
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
        /// Gets the markdown value.
        /// </summary>
        /// <returns>Markdown value.</returns>
        public async Task<string> GetValueAsync()
        {
            if ( !Initialized )
                return null;

            return await JSModule.GetValue( ElementId );
        }

        /// <summary>
        /// Sets the markdown value.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SetValueAsync( string value )
        {
            if ( !Initialized )
                return;

            await JSModule.SetValue( ElementId, value );
        }

        /// <summary>
        /// Updates the internal markdown value. This method should only be called internally!
        /// </summary>
        /// <param name="value">New value.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [JSInvokable]
        public Task UpdateInternalValue( string value )
        {
            Value = value;

            return ValueChanged.InvokeAsync( Value );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        protected override bool ShouldAutoGenerateId => true;

        protected JSMarkdownModule JSModule { get; private set; }

        /// <summary>
        /// Indicates if markdown editor is properly initialized.
        /// </summary>
        protected bool Initialized { get; set; }

        /// <summary>
        /// Gets or set the javascript runtime.
        /// </summary>
        [Inject] private IJSRuntime JSRuntime { get; set; }

        /// <summary>
        /// Gets or sets the version provider.
        /// </summary>
        [Inject] private IVersionProvider VersionProvider { get; set; }

        /// <summary>
        /// Gets or sets the markdown value.
        /// </summary>
        [Parameter] public string Value { get; set; }

        /// <summary>
        /// An event that occurs after the markdown value has changed.
        /// </summary>
        [Parameter] public EventCallback<string> ValueChanged { get; set; }

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
        [Parameter] public string[] HideIcons { get; set; } = new[] { "side-by-side", "fullscreen" };

        /// <summary>
        /// An array of icon names to show. Can be used to show specific icons hidden by default without
        /// completely customizing the toolbar.
        /// </summary>
        [Parameter] public string[] ShowIcons { get; set; } = new[] { "code", "table" };

        /// <summary>
        /// If set to false, hide the toolbar. Defaults to the array of icons.
        /// </summary>
        [Parameter] public object Toolbar { get; set; }

        /// <summary>
        /// If set to false, disable toolbar button tips. Defaults to true.
        /// </summary>
        [Parameter] public bool ToolbarTips { get; set; } = true;

        #endregion
    }
}
