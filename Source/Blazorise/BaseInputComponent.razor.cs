#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Base component for all the input component types.
    /// </summary>
    public abstract class BaseInputComponent<TValue> : BaseSizableComponent, IValidationInput
    {
        #region Members

        private Size size = Size.None;

        private bool readOnly;

        private bool disabled;

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            // link to the parent component
            if ( ParentValidation != null )
            {
                ParentValidation.InitializeInput( this );

                ParentValidation.ValidationStatusChanged += OnValidationStatusChanged;
            }

            base.OnInitialized();
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( ParentValidation != null )
                {
                    // To avoid leaking memory, it's important to detach any event handlers in Dispose()
                    ParentValidation.ValidationStatusChanged -= OnValidationStatusChanged;
                }
            }

            base.Dispose( disposing );
        }

        private void OnValidationStatusChanged( object sender, ValidationStatusChangedEventArgs e )
        {
            DirtyClasses();
            StateHasChanged();
        }

        /// <summary>
        /// Handles the parsing of an input value.
        /// </summary>
        /// <param name="value">Input value to be parsed.</param>
        /// <returns>Returns the awaitable task.</returns>
        protected async Task CurrentValueHandler( string value )
        {
            var empty = false;

            if ( string.IsNullOrEmpty( value ) )
            {
                empty = true;
                CurrentValue = default;
            }

            if ( !empty )
            {
                var result = await ParseValueFromStringAsync( value );

                if ( result.Success )
                {
                    CurrentValue = result.ParsedValue;
                }
            }

            // send the value to the validation for processing
            ParentValidation?.NotifyInputChanged();
        }

        protected abstract Task<ParseValue<TValue>> ParseValueFromStringAsync( string value );

        protected virtual string FormatValueAsString( TValue value )
            => value?.ToString();

        protected virtual object PrepareValueForValidation( TValue value )
            => value;

        /// <summary>
        /// Raises and event that handles the edit value of Text, Date, Numeric etc.
        /// </summary>
        /// <param name="value">New edit value.</param>
        protected abstract Task OnInternalValueChanged( TValue value );

        /// <summary>
        /// Sets focus on the input element, if it can be focused.
        /// </summary>
        /// <param name="scrollToElement">If true the browser should scroll the document to bring the newly-focused element into view.</param>
        public void Focus( bool scrollToElement = true )
        {
            _ = JSRunner.Focus( ElementRef, ElementId, scrollToElement );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public virtual object ValidationValue => InternalValue;

        /// <summary>
        /// Gets or sets the internal edit value.
        /// </summary>
        /// <remarks>
        /// The reason for this to be abstract is so that input components can have
        /// their own specialized parameters that can be binded(Text, Date, Value etc.)
        /// </remarks>
        protected abstract TValue InternalValue { get; set; }

        protected TValue CurrentValue
        {
            get => InternalValue;
            set
            {
                if ( !EqualityComparer<TValue>.Default.Equals( value, InternalValue ) )
                {
                    InternalValue = value;
                    _ = OnInternalValueChanged( value );
                }
            }
        }

        protected string CurrentValueAsString
        {
            get => FormatValueAsString( CurrentValue );
            set
            {
                _ = CurrentValueHandler( value );
            }
        }

        /// <summary>
        /// Sets the size of the input control.
        /// </summary>
        [Parameter]
        public Size Size
        {
            get => size;
            set
            {
                size = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Add the readonly boolean attribute on an input to prevent modification of the input’s value.
        /// </summary>
        [Parameter]
        public bool ReadOnly
        {
            get => readOnly;
            set
            {
                readOnly = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Add the disabled boolean attribute on an input to prevent user interactions and make it appear lighter.
        /// </summary>
        [Parameter]
        public bool Disabled
        {
            get => disabled;
            set
            {
                disabled = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Placeholder for validation messages.
        /// </summary>
        [Parameter] public RenderFragment Feedback { get; set; }

        /// <summary>
        /// Input content.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Parent validation container.
        /// </summary>
        [CascadingParameter] protected Validation ParentValidation { get; set; }

        #endregion
    }
}
