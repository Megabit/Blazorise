﻿#region Using directives
using System.Threading.Tasks;
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

                await JSRuntime.InvokeVoidAsync( "blazoriseMarkdown.initialize", dotNetObjectRef, ElementId, Value );

                Initialized = true;
            }
        }

        /// <inheritdoc/>
        protected override async ValueTask DisposeAsync( bool disposing )
        {
            if ( disposing )
            {
                await JSRuntime.InvokeVoidAsync( "blazoriseMarkdown.destroy", ElementId );

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

            return await JSRuntime.InvokeAsync<string>( "blazoriseMarkdown.getValue", ElementId );
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

            await JSRuntime.InvokeAsync<string>( "blazoriseMarkdown.setValue", ElementId, value );
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

        /// <summary>
        /// Indicates if markdown editor is properly initialized.
        /// </summary>
        protected bool Initialized { get; set; }

        /// <summary>
        /// Gets or set the javascript runtime.
        /// </summary>
        [Inject] public IJSRuntime JSRuntime { get; set; }

        /// <summary>
        /// Gets or sets the markdown value.
        /// </summary>
        [Parameter] public string Value { get; set; }

        /// <summary>
        /// An event that occurs after the markdown value has changed.
        /// </summary>
        [Parameter] public EventCallback<string> ValueChanged { get; set; }

        #endregion
    }
}
