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
    public abstract class BaseTextInput<TValue> : BaseInputComponent<TValue>, ISelectableComponent, IDisposable
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
                inputValueDebouncer = new( DelayTextOnKeyPressIntervalValue );
                inputValueDebouncer.Debounced += OnInputValueDebounced;
            }

            base.OnInitialized();
        }

        /// <inheritdoc/>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                ReleaseResources();
            }

            base.Dispose( disposing );
        }

        /// <inheritdoc/>
        protected override ValueTask DisposeAsync( bool disposing )
        {
            if ( disposing )
            {
                ReleaseResources();
            }

            return base.DisposeAsync( disposing );
        }

        /// <summary>
        /// Shared code to dispose of any internal resources.
        /// </summary>
        protected override void ReleaseResources()
        {
            if ( inputValueDebouncer != null )
            {
                inputValueDebouncer.Debounced -= OnInputValueDebounced;
                inputValueDebouncer = null;
            }

            base.ReleaseResources();
        }

        /// <summary>
        /// Handler for @onchange event.
        /// </summary>
        /// <param name="eventArgs">Information about the changed event.</param>
        /// <returns>Returns awaitable task</returns>
        protected virtual Task OnChangeHandler( ChangeEventArgs eventArgs )
        {
            if ( !IsImmediate )
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
        protected virtual Task OnInputHandler( ChangeEventArgs eventArgs )
        {
            if ( IsImmediate )
            {
                var value = eventArgs?.Value?.ToString();
                if ( IsDelayTextOnKeyPress )
                {
                    inputValueDebouncer?.Update( value );
                }
                else
                {
                    return CurrentValueHandler( value );
                }
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        protected override Task OnKeyPressHandler( KeyboardEventArgs eventArgs )
        {
            if ( IsImmediate
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
            if ( IsImmediate
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

        /// <inheritdoc/>
        public virtual Task Select( bool focus = true )
        {
            return JSUtilitiesModule.Select( ElementRef, ElementId, focus ).AsTask();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns true if internal value should be updated with each key press.
        /// </summary>
        protected bool IsImmediate
            => Immediate.GetValueOrDefault( Options?.Immediate ?? true );

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
        /// the name of the event for the input element.
        /// </summary>
        protected string BindValueEventName
            => IsImmediate ? "oninput" : "onchange";

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
        /// Note that setting this will override global settings in <see cref="BlazoriseOptions.Immediate"/>.
        /// </remarks>
        [Parameter] public bool? Immediate { get; set; }

        /// <summary>
        /// If true the entered text will be slightly delayed before submitting it to the internal value.
        /// </summary>
        [Parameter] public bool? DelayTextOnKeyPress { get; set; }

        /// <summary>
        /// Interval in milliseconds that entered text will be delayed from submitting to the internal value.
        /// </summary>
        [Parameter] public int? DelayTextOnKeyPressInterval { get; set; }

        #endregion
    }
}
