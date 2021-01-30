#region Using directives
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Component that allows you to display and edit multi-line text.
    /// </summary>
    public partial class MemoEdit : BaseInputComponent<string>
    {
        #region Members

        private ValueDebouncer inputValueDebouncer;

        #endregion

        #region Methods

        public override async Task SetParametersAsync( ParameterView parameters )
        {
            await base.SetParametersAsync( parameters );

            if ( ParentValidation != null )
            {
                if ( parameters.TryGetValue<Expression<Func<string>>>( nameof( TextExpression ), out var expression ) )
                    ParentValidation.InitializeInputExpression( expression );

                InitializeValidation();
            }
        }

        protected override void OnInitialized()
        {
            if ( IsDelayTextOnKeyPress )
            {
                inputValueDebouncer = new ValueDebouncer( DelayTextOnKeyPressIntervalValue );
                inputValueDebouncer.Debounced += OnInputValueDebounced;
            }

            base.OnInitialized();
        }

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.MemoEdit() );
            builder.Append( ClassProvider.MemoEditValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

            base.BuildClasses( builder );
        }

        /// <inheritdoc/>
        protected override Task OnInternalValueChanged( string value )
        {
            return TextChanged.InvokeAsync( value );
        }

        /// <inheritdoc/>
        protected override Task<ParseValue<string>> ParseValueFromStringAsync( string value )
        {
            return Task.FromResult( new ParseValue<string>( true, value, null ) );
        }

        /// <inheritdoc/>
        protected Task OnChangeHandler( ChangeEventArgs e )
        {
            if ( !IsChangeTextOnKeyPress )
            {
                return CurrentValueHandler( e?.Value?.ToString() );
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        protected async Task OnInputHandler( ChangeEventArgs e )
        {
            if ( IsChangeTextOnKeyPress )
            {
                if ( IsDelayTextOnKeyPress )
                {
                    inputValueDebouncer?.Update( e?.Value?.ToString() );
                }
                else
                {
                    var caret = await JSRunner.GetCaret( ElementRef );

                    await CurrentValueHandler( e?.Value?.ToString() );

                    await JSRunner.SetCaret( ElementRef, caret );
                }
            }
        }

        /// <inheritdoc/>
        protected override Task OnKeyPressHandler( KeyboardEventArgs eventArgs )
        {
            if ( IsChangeTextOnKeyPress
                && IsDelayTextOnKeyPress
                && ( eventArgs?.Key?.Equals( "Enter", StringComparison.OrdinalIgnoreCase ) ?? false ) )
            {
                inputValueDebouncer?.Flush();
            }

            return base.OnKeyPressHandler( eventArgs );
        }

        /// <inheritdoc/>
        protected override Task OnBlurHandler( FocusEventArgs eventArgs )
        {
            if ( IsChangeTextOnKeyPress
                && IsDelayTextOnKeyPress )
            {
                inputValueDebouncer?.Flush();
            }

            return base.OnBlurHandler( eventArgs );
        }

        /// <summary>
        /// Event raised after the delayed value time has expired.
        /// </summary>
        /// <param name="sender">Object that raised an event.</param>
        /// <param name="value">Latest received value.</param>
        private void OnInputValueDebounced( object sender, string value )
        {
            InvokeAsync( async () =>
            {
                await CurrentValueHandler( value );
            } );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        protected override string InternalValue { get => Text; set => Text = value; }

        /// <inheritdoc/>
        protected override string DefaultValue => string.Empty;

        /// <summary>
        /// Returns true if internal value should be updated with each key press.
        /// </summary>
        protected bool IsChangeTextOnKeyPress
            => ChangeTextOnKeyPress.GetValueOrDefault( Options?.ChangeTextOnKeyPress ?? true );

        /// <summary>
        /// Returns true if updating of internal value should be delayed.
        /// </summary>
        protected bool IsDelayTextOnKeyPress
            => DelayTextOnKeyPress.GetValueOrDefault( Options?.DelayTextOnKeyPress ?? false );

        /// <summary>
        /// Time in milliseconds by which internal value update should be delayed.
        /// </summary>
        protected int DelayTextOnKeyPressIntervalValue
            => DelayTextOnKeyPressInterval.GetValueOrDefault( Options?.DelayTextOnKeyPressInterval ?? 300 );

        /// <summary>
        /// Sets the placeholder for the empty text.
        /// </summary>
        [Parameter] public string Placeholder { get; set; }

        /// <summary>
        /// Gets or sets the text inside the input field.
        /// </summary>
        [Parameter] public string Text { get; set; }

        /// <summary>
        /// Occurs after text has changed.
        /// </summary>
        [Parameter] public EventCallback<string> TextChanged { get; set; }

        /// <summary>
        /// Gets or sets an expression that identifies the text value.
        /// </summary>
        [Parameter] public Expression<Func<string>> TextExpression { get; set; }

        /// <summary>
        /// Specifies the maximum number of characters allowed in the input element.
        /// </summary>
        [Parameter] public int? MaxLength { get; set; }

        /// <summary>
        /// Specifies the number lines in the input element.
        /// </summary>
        [Parameter] public int? Rows { get; set; }

        /// <summary>
        /// If true the text in will be changed after each key press.
        /// </summary>
        /// <remarks>
        /// Note that setting this will override global settings in <see cref="BlazoriseOptions.ChangeTextOnKeyPress"/>.
        /// </remarks>
        [Parameter] public bool? ChangeTextOnKeyPress { get; set; }

        /// <summary>
        /// If true the entered text will be slightly delayed before submiting it to the internal value.
        /// </summary>
        [Parameter] public bool? DelayTextOnKeyPress { get; set; }

        /// <summary>
        /// Interval in milliseconds that entered text will be delayed from submiting to the internal value.
        /// </summary>
        [Parameter] public int? DelayTextOnKeyPressInterval { get; set; }

        #endregion
    }
}
