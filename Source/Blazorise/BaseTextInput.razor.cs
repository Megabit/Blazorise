#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Utils;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Base component for inputs that are text-based.
    /// </summary>
    /// <typeparam name="TValue">Editable value type.</typeparam>
    public abstract class BaseTextInput<TValue> : BaseInputComponent<TValue>
    {
        #region Members

        private Color color;

        private ValueDelayer inputValueDelayer;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TextEdit( Plaintext ) );
            builder.Append( ClassProvider.TextEditColor( Color ), Color != Color.None );
            builder.Append( ClassProvider.TextEditSize( Size ), Size != Size.None );
            builder.Append( ClassProvider.TextEditValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

            base.BuildClasses( builder );
        }

        protected override void OnInitialized()
        {
            if ( IsDelayTextOnKeyPress )
            {
                inputValueDelayer = new ValueDelayer( DelayTextOnKeyPressIntervalValue );
                inputValueDelayer.Delayed += OnInputValueDelayed;
            }

            if ( ParentValidation != null )
            {
                ParentValidation.InitializeInputPattern( Pattern );
            }

            base.OnInitialized();
        }

        protected override void Dispose( bool disposing )
        {
            if ( inputValueDelayer != null )
            {
                inputValueDelayer.Delayed -= OnInputValueDelayed;
                inputValueDelayer = null;
            }

            base.Dispose( disposing );
        }

        protected virtual Task OnChangeHandler( ChangeEventArgs e )
        {
            if ( !IsChangeTextOnKeyPress )
            {
                return CurrentValueHandler( e?.Value?.ToString() );
            }

            return Task.CompletedTask;
        }

        protected virtual async Task OnInputHandler( ChangeEventArgs e )
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

        private void OnInputValueDelayed( object sender, string value )
        {
            InvokeAsync( async () =>
            {
                await CurrentValueHandler( value );
            } );
        }

        #endregion

        #region Properties

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
        /// Sets the class to remove the default form field styling and preserve the correct margin and padding.
        /// </summary>
        [Parameter] public bool Plaintext { get; set; }

        /// <summary>
        /// Sets the input text color.
        /// </summary>
        [Parameter]
        public Color Color
        {
            get => color;
            set
            {
                color = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// The pattern attribute specifies a regular expression that the input element's value is checked against on form validation.
        /// </summary>
        [Parameter] public string Pattern { get; set; }

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
