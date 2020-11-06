#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Utils;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class MemoEdit : BaseInputComponent<string>
    {
        #region Members

        private ValueDelayer inputValueDelayer;

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
                inputValueDelayer = new ValueDelayer( DelayTextOnKeyPressIntervalValue );
                inputValueDelayer.Delayed += OnInputValueDelayed;
            }

            base.OnInitialized();
        }

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.MemoEdit() );
            builder.Append( ClassProvider.MemoEditValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

            base.BuildClasses( builder );
        }

        protected Task OnChangeHandler( ChangeEventArgs e )
        {
            if ( !IsChangeTextOnKeyPress )
            {
                return CurrentValueHandler( e?.Value?.ToString() );
            }

            return Task.CompletedTask;
        }

        protected async Task OnInputHandler( ChangeEventArgs e )
        {
            if ( IsChangeTextOnKeyPress )
            {
                if ( IsDelayTextOnKeyPress )
                {
                    inputValueDelayer?.Update( e?.Value?.ToString() );
                }
                else
                {
                    var caret = await JSRunner.GetCaret( ElementRef );

                    await CurrentValueHandler( e?.Value?.ToString() );

                    await JSRunner.SetCaret( ElementRef, caret );
                }
            }
        }

        protected override Task OnInternalValueChanged( string value )
        {
            return TextChanged.InvokeAsync( value );
        }

        protected override Task<ParseValue<string>> ParseValueFromStringAsync( string value )
        {
            return Task.FromResult( new ParseValue<string>( true, value, null ) );
        }

        private void OnInputValueDelayed( object sender, string value )
        {
            InvokeAsync( async () =>
            {
                await CurrentValueHandler( value );
            } );
        }

        #endregion

        #region Properties

        protected override string InternalValue { get => Text; set => Text = value; }

        private bool IsChangeTextOnKeyPress
           => ChangeTextOnKeyPress.GetValueOrDefault( Options?.ChangeTextOnKeyPress ?? true );

        private bool IsDelayTextOnKeyPress
           => DelayTextOnKeyPress.GetValueOrDefault( Options?.DelayTextOnKeyPress ?? false );

        private int DelayTextOnKeyPressIntervalValue
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
