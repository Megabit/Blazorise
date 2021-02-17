#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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

        private ValueDebouncer inputValueDebouncer;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            if ( IsDelayTextOnKeyPress )
            {
                inputValueDebouncer = new ValueDebouncer( DelayTextOnKeyPressIntervalValue );
                inputValueDebouncer.Debounced += OnInputValueDebounced;
            }

            base.OnInitialized();
        }

        /// <inheritdoc/>
        protected override void Dispose( bool disposing )
        {
            if ( inputValueDebouncer != null )
            {
                inputValueDebouncer.Debounced -= OnInputValueDebounced;
                inputValueDebouncer = null;
            }

            base.Dispose( disposing );
        }

        /// <summary>
        /// Handler for @onchange event.
        /// </summary>
        /// <param name="eventArgs">Information about the changed event.</param>
        /// <returns>Returns awaitable task</returns>
        protected virtual Task OnChangeHandler( ChangeEventArgs eventArgs )
        {
            if ( !IsChangeTextOnKeyPress )
            {
                return CurrentValueHandler( eventArgs?.Value?.ToString() );
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Handler for @oninput event.
        /// </summary>
        /// <param name="eventArgs">Information about the changed event.</param>
        /// <returns>Returns awaitable task</returns>
        protected virtual async Task OnInputHandler( ChangeEventArgs eventArgs )
        {
            if ( IsChangeTextOnKeyPress )
            {
                if ( IsDelayTextOnKeyPress )
                {
                    inputValueDebouncer?.Update( eventArgs?.Value?.ToString() );
                }
                else
                {
                    var caret = await JSRunner.GetCaret( ElementRef );

                    await CurrentValueHandler( eventArgs?.Value?.ToString() );

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
            InvokeAsync( () => CurrentValueHandler( value ) );
        }

        #endregion

        #region Properties

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
