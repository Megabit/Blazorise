#region Using directives
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Format input text content when you are typing.
    /// </summary>
    public partial class InputMask : BaseTextInput<string>, IAsyncDisposable
    {
        #region Methods

        /// <inheritdoc/>
        public override async Task SetParametersAsync( ParameterView parameters )
        {
            // Let blazor do its thing!
            await base.SetParametersAsync( parameters );

            if ( ParentValidation != null )
            {
                if ( parameters.TryGetValue<Expression<Func<string>>>( nameof( ValueExpression ), out var expression ) )
                    await ParentValidation.InitializeInputExpression( expression );

                if ( parameters.TryGetValue<string>( nameof( Pattern ), out var pattern ) )
                {
                    // make sure we get the newest value
                    var value = parameters.TryGetValue<string>( nameof( Value ), out var paramValue )
                        ? paramValue
                        : InternalValue;

                    await ParentValidation.InitializeInputPattern( pattern, value );
                }

                await InitializeValidation();
            }
        }

        /// <inheritdoc/>
        protected override async Task OnFirstAfterRenderAsync()
        {
            await JSModule.Initialize( ElementRef, ElementId, new
            {
                Mask,
                Regex,
                Placeholder,
                ShowMaskOnFocus,
                ShowMaskOnHover,
                Disabled,
                ReadOnly,
            } );

            await base.OnFirstAfterRenderAsync();
        }

        /// <inheritdoc/>
        protected override async ValueTask DisposeAsync( bool disposing )
        {
            if ( disposing && Rendered )
            {
                await JSModule.SafeDestroy( ElementRef, ElementId );
            }

            await base.DisposeAsync( disposing );
        }

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.InputMask( Plaintext ) );
            builder.Append( ClassProvider.InputMaskSize( ThemeSize ), ThemeSize != Blazorise.Size.None );
            builder.Append( ClassProvider.InputMaskColor( Color ), Color != Color.None );
            builder.Append( ClassProvider.InputMaskValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

            base.BuildClasses( builder );
        }

        /// <inheritdoc/>
        protected override Task OnChangeHandler( ChangeEventArgs e )
        {
            return CurrentValueHandler( e?.Value?.ToString() );
        }

        /// <inheritdoc/>
        protected override Task OnInternalValueChanged( string value )
        {
            return ValueChanged.InvokeAsync( value );
        }

        /// <inheritdoc/>
        protected override Task<ParseValue<string>> ParseValueFromStringAsync( string value )
        {
            return Task.FromResult( new ParseValue<string>( true, value, null ) );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        protected override bool ShouldAutoGenerateId => true;

        /// <inheritdoc/>
        protected override string InternalValue { get => Value; set => Value = value; }

        /// <summary>
        /// Gets or sets the <see cref="IJSInputMaskModule"/> instance.
        /// </summary>
        [Inject] public IJSInputMaskModule JSModule { get; set; }

        /// <summary>
        /// Gets or sets the input time value.
        /// </summary>
        [Parameter] public string Value { get; set; }

        /// <summary>
        /// Occurs when the time has changed.
        /// </summary>
        [Parameter] public EventCallback<string> ValueChanged { get; set; }

        /// <summary>
        /// Gets or sets an expression that identifies the time field.
        /// </summary>
        [Parameter] public Expression<Func<string>> ValueExpression { get; set; }

        /// <summary>
        /// The mask to use for the input.
        /// </summary>
        [Parameter] public string Mask { get; set; }

        /// <summary>
        /// Use a regular expression as a mask.
        /// </summary>
        [Parameter] public string Regex { get; set; }

        /// <summary>
        /// Shows the mask when the input gets focus. (default = true)
        /// </summary>
        [Parameter] public bool ShowMaskOnFocus { get; set; } = true;

        /// <summary>
        /// Shows the mask when hovering the mouse. (default = true)
        /// </summary>
        [Parameter] public bool ShowMaskOnHover { get; set; } = true;

        #endregion
    }
}
