#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Base component for all the input component types.
    /// </summary>
    public abstract class BaseInputComponent<TValue> : BaseComponent, IValidationInput, IFocusableComponent
    {
        #region Members

        private Size size = Size.None;

        private bool readOnly;

        private bool disabled;

        /// <summary>
        /// Internal value for autofocus flag.
        /// </summary>
        private bool autofocus;

        private bool validationInitialized;

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override async Task SetParametersAsync( ParameterView parameters )
        {
            await base.SetParametersAsync( parameters );

            // For modals we need to make sure that autofocus is applied every time the modal is opened.
            if ( parameters.TryGetValue<bool>( nameof( Autofocus ), out var autofocus ) && this.autofocus != autofocus )
            {
                this.autofocus = autofocus;

                if ( autofocus )
                {
                    if ( ParentModal != null )
                    {
                        ParentModal.NotifyFocusableComponentInitialized( this );
                    }
                    else
                    {
                        ExecuteAfterRender( async () =>
                        {
                            await FocusAsync();
                        } );
                    }
                }
                else
                {
                    if ( ParentModal != null )
                    {
                        ParentModal.NotifyFocusableComponentRemoved( this );
                    }
                }
            }
        }

        /// <inheritdoc/>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( ParentValidation != null )
                {
                    // To avoid leaking memory, it's important to detach any event handlers in Dispose()
                    ParentValidation.ValidationStatusChanged -= OnValidationStatusChanged;
                }

                if ( ParentModal != null )
                {
                    ParentModal.NotifyFocusableComponentRemoved( this );
                }
            }

            base.Dispose( disposing );
        }

        /// <summary>
        /// Initializes input component for validation.
        /// </summary>
        protected void InitializeValidation()
        {
            if ( validationInitialized )
                return;

            // link to the parent component
            ParentValidation.InitializeInput( this );

            ParentValidation.ValidationStatusChanged += OnValidationStatusChanged;

            validationInitialized = true;
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
                CurrentValue = DefaultValue;
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
            ParentValidation?.NotifyInputChanged<TValue>( default );
        }

        /// <summary>
        /// Parses a string value and convert it to a <see cref="TValue"/>.
        /// </summary>
        /// <param name="value">A string value to convert.</param>
        /// <returns>Returns the result of parse operation.</returns>
        protected abstract Task<ParseValue<TValue>> ParseValueFromStringAsync( string value );

        /// <summary>
        /// Formats the supplied value to it's valid string representation.
        /// </summary>
        /// <param name="value">Value to format.</param>
        /// <returns>Reterns value formated as string.</returns>
        protected virtual string FormatValueAsString( TValue value )
            => value?.ToString();

        /// <summary>
        /// Prepares the right value to be sent for validation.
        /// </summary>
        /// <remarks>
        /// In some special cases we need to know what is the right value of the underline component.
        /// Like for example for <see cref="Select{TValue}"/> component where we can have value represented as
        /// a single or multiple value, depending on the context where it is used.
        /// </remarks>
        /// <param name="value">Value to prepare for valudation.</param>
        /// <returns>Returns the value that is going to be validated.</returns>
        protected virtual object PrepareValueForValidation( TValue value )
            => value;

        /// <summary>
        /// Raises and event that handles the edit value of Text, Date, Numeric etc.
        /// </summary>
        /// <param name="value">New edit value.</param>
        protected abstract Task OnInternalValueChanged( TValue value );

        /// <inheritdoc/>
        public void Focus( bool scrollToElement = true )
        {
            InvokeAsync( () => FocusAsync( scrollToElement ) );
        }

        /// <inheritdoc/>
        public async Task FocusAsync( bool scrollToElement = true )
        {
            await JSRunner.Focus( ElementRef, ElementId, scrollToElement );
        }

        /// <summary>
        /// Handler for @onkeydown event.
        /// </summary>
        /// <param name="eventArgs">Information about the keyboard down event.</param>
        /// <returns>Returns awaitable task</returns>
        protected virtual Task OnKeyDownHandler( KeyboardEventArgs eventArgs )
        {
            return KeyDown.InvokeAsync( eventArgs );
        }

        /// <summary>
        /// Handler for @onkeypress event.
        /// </summary>
        /// <param name="eventArgs">Information about the keyboard pressed event.</param>
        /// <returns>Returns awaitable task</returns>
        protected virtual Task OnKeyPressHandler( KeyboardEventArgs eventArgs )
        {
            return KeyPress.InvokeAsync( eventArgs );
        }

        /// <summary>
        /// Handler for @onkeyup event.
        /// </summary>
        /// <param name="eventArgs">Information about the keyboard up event.</param>
        /// <returns>Returns awaitable task</returns>
        protected virtual Task OnKeyUpHandler( KeyboardEventArgs eventArgs )
        {
            return KeyUp.InvokeAsync( eventArgs );
        }

        /// <summary>
        /// Handler for @onblur event.
        /// </summary>
        /// <param name="eventArgs">Information about the focus event.</param>
        /// <returns>Returns awaitable task</returns>
        protected virtual Task OnBlurHandler( FocusEventArgs eventArgs )
        {
            return Blur.InvokeAsync( eventArgs );
        }

        /// <summary>
        /// Handler for @onfocus event.
        /// </summary>
        /// <param name="eventArgs">Information about the focus event.</param>
        /// <returns>Returns awaitable task</returns>
        protected virtual Task OnFocusHandler( FocusEventArgs eventArgs )
        {
            return OnFocus.InvokeAsync( eventArgs );
        }

        /// <summary>
        /// Handler for @onfocusin event.
        /// </summary>
        /// <param name="eventArgs">Information about the focus event.</param>
        /// <returns>Returns awaitable task</returns>
        protected virtual Task OnFocusInHandler( FocusEventArgs eventArgs )
        {
            return FocusIn.InvokeAsync( eventArgs );
        }

        /// <summary>
        /// Handler for @onfocusout event.
        /// </summary>
        /// <param name="eventArgs">Information about the focus event.</param>
        /// <returns>Returns awaitable task</returns>
        protected virtual Task OnFocusOutHandler( FocusEventArgs eventArgs )
        {
            return FocusOut.InvokeAsync( eventArgs );
        }

        /// <summary>
        /// Forces the <see cref="Validation"/>(if any is used) to re-validate with the new custom or internal value.
        /// </summary>
        public void Revalidate()
        {
            ParentValidation?.NotifyInputChanged<TValue>( default );
        }

        /// <summary>
        /// Handler for validation status change event.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="eventArgs">Information about the validation status.</param>
        protected async void OnValidationStatusChanged( object sender, ValidationStatusChangedEventArgs eventArgs )
        {
            DirtyClasses();
            await InvokeAsync( StateHasChanged );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        protected override bool ShouldAutoGenerateId => true;

        /// <inheritdoc/>
        public virtual object ValidationValue => CustomValidationValue != null
            ? CustomValidationValue.Invoke()
            : InternalValue;

        /// <summary>
        /// Returns true if input belong to a <see cref="FieldBody"/>.
        /// </summary>
        protected bool ParentIsFieldBody => ParentFieldBody != null;

        /// <summary>
        /// Returns the default value for the <typeparamref name="TValue"/> type.
        /// </summary>
        protected virtual TValue DefaultValue => default;

        /// <summary>
        /// Gets or sets the internal edit value.
        /// </summary>
        /// <remarks>
        /// The reason for this to be abstract is so that input components can have
        /// their own specialized parameters that can be binded(Text, Date, Value etc.)
        /// </remarks>
        protected abstract TValue InternalValue { get; set; }

        /// <summary>
        /// Gets or sets the current input value.
        /// </summary>
        protected TValue CurrentValue
        {
            get => InternalValue;
            set
            {
                if ( !value.IsEqual( InternalValue ) )
                {
                    InternalValue = value;
                    InvokeAsync( () => OnInternalValueChanged( value ) );
                }
            }
        }

        /// <summary>
        /// Gets or sets the current input value represented as a string.
        /// </summary>
        protected string CurrentValueAsString
        {
            get => FormatValueAsString( CurrentValue );
            set
            {
                InvokeAsync( () => CurrentValueHandler( value ) );
            }
        }

        /// <summary>
        /// Holds the information about the Blazorise global options.
        /// </summary>
        [Inject] protected BlazoriseOptions Options { get; set; }

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
        /// Set's the focus to the component after the rendering is done.
        /// </summary>
        [Parameter] public bool Autofocus { get; set; }

        /// <summary>
        /// Placeholder for validation messages.
        /// </summary>
        [Parameter] public RenderFragment Feedback { get; set; }

        /// <summary>
        /// Input content.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Occurs when a key is pressed down while the control has focus.
        /// </summary>
        [Parameter] public EventCallback<KeyboardEventArgs> KeyDown { get; set; }

        /// <summary>
        /// Occurs when a key is pressed while the control has focus.
        /// </summary>
        [Parameter] public EventCallback<KeyboardEventArgs> KeyPress { get; set; }

        /// <summary>
        /// Occurs when a key is released while the control has focus.
        /// </summary>
        [Parameter] public EventCallback<KeyboardEventArgs> KeyUp { get; set; }

        /// <summary>
        /// The blur event fires when an element has lost focus.
        /// </summary>
        [Parameter] public EventCallback<FocusEventArgs> Blur { get; set; }

        /// <summary>
        /// Occurs when the input box gains or loses focus.
        /// </summary>
        [Parameter] public EventCallback<FocusEventArgs> OnFocus { get; set; }

        /// <summary>
        /// Occurs when the input box gains focus.
        /// </summary>
        [Parameter] public EventCallback<FocusEventArgs> FocusIn { get; set; }

        /// <summary>
        /// Occurs when the input box loses focus.
        /// </summary>
        [Parameter] public EventCallback<FocusEventArgs> FocusOut { get; set; }

        /// <summary>
        /// If defined, indicates that its element can be focused and can participates in sequential keyboard navigation.
        /// </summary>
        [Parameter] public int? TabIndex { get; set; }

        /// <summary>
        /// Used to provide custom validation value on which the validation will be processed with
        /// the <see cref="Validation.Validator"/> handler.
        /// </summary>
        /// <remarks>
        /// Should be used carefully as it's only meant for some special cases when input is used
        /// in a wrapper component, like Autocomplete or SelectList.
        /// </remarks>
        [Parameter] public Func<TValue> CustomValidationValue { get; set; }

        /// <summary>
        /// Parent validation container.
        /// </summary>
        [CascadingParameter] protected Validation ParentValidation { get; set; }

        /// <summary>
        /// Parent field body.
        /// </summary>
        [CascadingParameter] protected FieldBody ParentFieldBody { get; set; }

        /// <summary>
        /// Parent modal dialog.
        /// </summary>
        [CascadingParameter] protected Modal ParentModal { get; set; }

        #endregion
    }
}
