#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    public partial class ColorPicker : BaseInputComponent<string>, ISelectableComponent
    {
        #region Members

        /// <summary>
        /// Object reference that can be accessed through the JSInterop.
        /// </summary>
        private DotNetObjectReference<ColorPicker> dotNetObjectRef;

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override async Task SetParametersAsync( ParameterView parameters )
        {
            var colorChanged = parameters.TryGetValue<string>( nameof( Color ), out var color ) && !Color.IsEqual( color );

            if ( colorChanged )
            {
                await CurrentValueHandler( color );

                if ( Rendered )
                {
                    ExecuteAfterRender( async () => await JSRunner.UpdateColorPickerValue( ElementRef, ElementId, color ) );
                }
            }

            await base.SetParametersAsync( parameters );

            if ( ParentValidation != null )
            {
                if ( parameters.TryGetValue<Expression<Func<string>>>( nameof( ColorExpression ), out var expression ) )
                    await ParentValidation.InitializeInputExpression( expression );

                await InitializeValidation();
            }
        }

        /// <inheritdoc/>
        protected override async Task OnFirstAfterRenderAsync()
        {
            dotNetObjectRef ??= CreateDotNetObjectRef( this );

            await JSRunner.InitializeColorPicker( dotNetObjectRef, ElementRef, ElementId, new
            {
                Default = Color,
                Swatches,
            } );

            await base.OnFirstAfterRenderAsync();
        }

        /// <inheritdoc/>
        protected override async ValueTask DisposeAsync( bool disposing )
        {
            if ( disposing )
            {
                if ( Rendered )
                {
                    var task = JSRunner.DestroyColorPicker( ElementRef, ElementId );

                    try
                    {
                        await task;
                    }
                    catch when ( task.IsCanceled )
                    {
                    }
                }
            }

            await base.DisposeAsync( disposing );
        }

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.ColorPicker() );
            builder.Append( ClassProvider.ColorPickerSize( ThemeSize ), ThemeSize != Blazorise.Size.None );

            base.BuildClasses( builder );
        }

        /// <summary>
        /// Handles the input onchange event.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected Task OnChangeHandler( ChangeEventArgs eventArgs )
        {
            return CurrentValueHandler( eventArgs?.Value?.ToString() );
        }

        /// <inheritdoc/>
        protected override Task OnInternalValueChanged( string value )
        {
            return ColorChanged.InvokeAsync( value );
        }

        /// <inheritdoc/>
        protected override string FormatValueAsString( string value )
        {
            return value;
        }

        /// <inheritdoc/>
        protected override Task<ParseValue<string>> ParseValueFromStringAsync( string value )
        {
            return Task.FromResult( new ParseValue<string>( true, value, null ) );
        }

        /// <inheritdoc/>
        public virtual Task Select( bool focus = true )
        {
            return JSRunner.Select( ElementRef, ElementId, focus ).AsTask();
        }

        /// <summary>
        /// Updated the <see cref="ColorPicker"/> with the new value.
        /// </summary>
        /// <param name="value">New color value.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [JSInvokable]
        public Task SetValue( string value )
        {
            if ( Color.IsEqual( value ) )
                return Task.CompletedTask;

            Color = value;

            return ColorChanged.InvokeAsync( Color );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        protected override string InternalValue { get => Color; set => Color = value; }

        /// <summary>
        /// Gets or sets the input color value.
        /// </summary>
        [Parameter] public string Color { get; set; }

        /// <summary>
        /// Optional color swatches. When null, swatches are disabled.
        /// </summary>
        [Parameter] public string[] Swatches { get; set; }

        /// <summary>
        /// Occurs when the color has changed.
        /// </summary>
        [Parameter] public EventCallback<string> ColorChanged { get; set; }

        /// <summary>
        /// Gets or sets an expression that identifies the color value.
        /// </summary>
        [Parameter] public Expression<Func<string>> ColorExpression { get; set; }

        #endregion
    }
}
