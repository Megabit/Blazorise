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
    public abstract class BaseInputComponent<TValue> : BaseSizableComponent
    {
        #region Members

        private Size size = Size.None;

        private bool isReadonly;

        private bool isDisabled;

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            // link to the parent component
            if ( ParentValidation != null )
            {
                ParentValidation.InitInputValue( PrepareValueForValidation( InternalValue ) );

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
            ParentValidation?.UpdateInputValue( PrepareValueForValidation( CurrentValue ) );
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
        protected abstract void OnInternalValueChanged( TValue value );

        #endregion

        #region Properties

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
                    OnInternalValueChanged( value );
                }
            }
        }

        protected string CurrentValueAsString
            => FormatValueAsString( CurrentValue );

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
        public bool IsReadonly
        {
            get => isReadonly;
            set
            {
                isReadonly = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Add the disabled boolean attribute on an input to prevent user interactions and make it appear lighter.
        /// </summary>
        [Parameter]
        public bool IsDisabled
        {
            get => isDisabled;
            set
            {
                isDisabled = value;

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
        [CascadingParameter] public BaseValidation ParentValidation { get; set; }

        #endregion
    }
}
