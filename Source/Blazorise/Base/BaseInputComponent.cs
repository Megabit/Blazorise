#region Using directives
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Base component for all the input component types.
/// </summary>
/// <typeparam name="TValue">Editable value type.</typeparam>
/// <typeparam name="TClasses">Component-specific classes type.</typeparam>
/// <typeparam name="TStyles">Component-specific styles type.</typeparam>
public abstract class BaseInputComponent<TValue, TClasses, TStyles> : BaseComponent<TClasses, TStyles>, IValidationInput, IFocusableComponent, IDisposable
    where TClasses : ComponentClasses
    where TStyles : ComponentStyles
{
    #region Members

    /// <summary>
    /// Size of an input element.
    /// </summary>
    private Size? size;

    /// <summary>
    /// Specifies that an input field is read-only.
    /// </summary>
    private bool readOnly;

    /// <summary>
    /// Specifies that the input element should be disabled.
    /// </summary>
    private bool disabled;

    /// <summary>
    /// Internal value for autofocus flag.
    /// </summary>
    private bool autofocus;

    /// <summary>
    /// Flag that tells us validation is already being initialized so we don't do it more than once.
    /// </summary>
    private bool validationInitialized;

    /// <summary>
    /// Flag that tells us if the parameters have been initialized.
    /// </summary>
    private bool hasInitializedParameters;

    /// <summary>
    /// Holds the aria-invalid attribute value.
    /// </summary>
    private string ariaInvalid;

    /// <summary>
    /// Holds the aria-describedby attribute value.
    /// </summary>
    private string ariaDescribedBy;

    /// <summary>
    /// Tracks the previous field reference for event subscription.
    /// </summary>
    private Field previousParentField;

    /// <summary>
    /// Defines if need to generate field names for the input components.
    /// </summary>
    protected bool shouldGenerateFieldNames;

    /// <summary>
    /// Holds the formatted value of a binded field.
    /// </summary>
    protected string formattedValueExpression;

    /// <summary>
    /// Contains metadata about the parameter representing the value for the component.
    /// </summary>
    protected ComponentParameterInfo<TValue> paramValue;

    /// <summary>
    /// Contains metadata about the parameter representing the value expression for the component.
    /// </summary>
    protected ComponentParameterInfo<Expression<Func<TValue>>> paramValueExpression;

    /// <summary>
    /// Contains metadata about the parameter representing the autofocus for the component.
    /// </summary>
    protected ComponentParameterInfo<bool> paramAutofocus;

    #endregion

    #region Methods

    /// <summary>
    /// Captures parameter values synchronously so they remain available after awaits in the SetParametersAsync flow.
    /// </summary>
    /// <param name="parameters">The parameters that will be passed to the component.</param>
    protected virtual void CaptureParameters( ParameterView parameters )
    {
        // Capture synchronously since ParameterView is not safe after awaits.
        if ( Rendered )
            parameters.TryGetParameter( nameof( Value ), Value, IsSameAsInternalValue, out paramValue );
        else
            paramValue = new ComponentParameterInfo<TValue>( default );

        parameters.TryGetParameter( nameof( ValueExpression ), ValueExpression, out paramValueExpression );
        parameters.TryGetParameter( nameof( Autofocus ), Autofocus, out paramAutofocus );
    }

    /// <summary>
    /// Method called before setting the parameters.
    /// </summary>
    /// <param name="parameters">The parameters that will be passed to the component.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual Task OnBeforeSetParametersAsync( ParameterView parameters )
    {
        InitializeParameters();

        if ( Rendered )
        {
            if ( paramValue.Defined && paramValue.Changed )
            {
                ExecuteAfterRender( Revalidate );
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Method called after the parameters are set.
    /// </summary>
    /// <param name="parameters">The parameters that have been set on the component.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual async Task OnAfterSetParametersAsync( ParameterView parameters )
    {
        if ( ParentValidation is not null )
        {
            if ( paramValueExpression.Defined )
                await ParentValidation.InitializeInputExpression( paramValueExpression.Value );

            await InitializeValidation();
        }

        // For modals we need to make sure that autofocus is applied every time the modal is opened.
        if ( paramAutofocus.Defined && autofocus != paramAutofocus.Value )
        {
            autofocus = paramAutofocus.Value;

            if ( paramAutofocus.Value )
            {
                if ( ParentFocusableContainer is not null )
                {
                    ParentFocusableContainer.NotifyFocusableComponentInitialized( this );
                }
                else
                {
                    ExecuteAfterRender( () => Focus() );
                }
            }
            else
            {
                ParentFocusableContainer?.NotifyFocusableComponentRemoved( this );
            }
        }

        if ( ParentField != previousParentField )
        {
            if ( previousParentField is not null )
                previousParentField.HelpTextChanged -= OnHelpTextChanged;

            if ( ParentField is not null )
                ParentField.HelpTextChanged += OnHelpTextChanged;

            previousParentField = ParentField;
        }

        UpdateAriaAttributes();
    }

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        CaptureParameters( parameters );

        await OnBeforeSetParametersAsync( parameters );

        await base.SetParametersAsync( parameters );

        await OnAfterSetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if ( ThemeOptions is not null )
        {
            ThemeOptions.Changed += OnThemeOptionsChanged;
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
    protected virtual void ReleaseResources()
    {
        if ( ParentValidation is not null )
        {
            // To avoid leaking memory, it's important to detach any event handlers in Dispose()
            ParentValidation.ValidationStatusChanged -= OnValidationStatusChanged;
            ParentValidation.ValidationMessageChanged -= OnValidationMessageChanged;
        }

        if ( previousParentField is not null )
        {
            previousParentField.HelpTextChanged -= OnHelpTextChanged;
            previousParentField = null;
        }

        ParentFocusableContainer?.NotifyFocusableComponentRemoved( this );

        if ( ThemeOptions is not null )
        {
            ThemeOptions.Changed -= OnThemeOptionsChanged;
        }
    }

    /// <summary>
    /// Initializes parameters that are not going to be changed again.
    /// </summary>
    protected void InitializeParameters()
    {
        if ( hasInitializedParameters )
            return;

        // we only need to generate field names on the server side
        shouldGenerateFieldNames = !OperatingSystem.IsBrowser();

        hasInitializedParameters = true;
    }

    /// <summary>
    /// Initializes input component for validation.
    /// </summary>
    protected async Task InitializeValidation()
    {
        if ( validationInitialized )
            return;

        // link to the parent component
        await ParentValidation.InitializeInput( this );

        ParentValidation.ValidationStatusChanged += OnValidationStatusChanged;
        ParentValidation.ValidationMessageChanged += OnValidationMessageChanged;

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
        if ( ParentValidation is not null )
        {
            await ParentValidation.NotifyInputChanged<TValue>( default );
        }
    }

    /// <summary>
    /// Parses a string value and convert it to a <see cref="BaseInputComponent{TValue}"/>.
    /// </summary>
    /// <param name="value">A string value to convert.</param>
    /// <returns>Returns the result of parse operation.</returns>
    protected abstract Task<ParseValue<TValue>> ParseValueFromStringAsync( string value );

    /// <summary>
    /// Formats the supplied value to it's valid string representation.
    /// </summary>
    /// <param name="value">Value to format.</param>
    /// <returns>Returns value formatted as string.</returns>
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
    /// <param name="value">Value to prepare for validation.</param>
    /// <returns>Returns the value that is going to be validated.</returns>
    protected virtual object PrepareValueForValidation( TValue value )
        => value;

    /// <summary>
    /// Check if the internal value is same as the new value.
    /// </summary>
    /// <param name="value">Value to check against the internal value.</param>
    /// <returns>True if the internal value matched the supplied value.</returns>
    protected virtual bool IsSameAsInternalValue( TValue value ) => value.IsEqual( Value );

    /// <summary>
    /// Raises and event that handles the edit value of Text, Date, Numeric etc.
    /// </summary>
    /// <param name="value">New edit value.</param>
    protected virtual Task OnInternalValueChanged( TValue value )
    {
        return ValueChanged.InvokeAsync( value );
    }

    /// <inheritdoc/>
    public virtual Task Focus( bool scrollToElement = true )
    {
        return JSUtilitiesModule.Focus( ElementRef, ElementId, scrollToElement ).AsTask();
    }

    /// <summary>
    /// Handler for <c>@onkeydown</c> event.
    /// </summary>
    /// <param name="eventArgs">Information about the keyboard down event.</param>
    /// <returns>Returns awaitable task</returns>
    protected virtual Task OnKeyDownHandler( KeyboardEventArgs eventArgs )
    {
        return KeyDown.InvokeAsync( eventArgs );
    }

    /// <summary>
    /// Handler for <c>@onkeypress</c> event.
    /// </summary>
    /// <param name="eventArgs">Information about the keyboard pressed event.</param>
    /// <returns>Returns awaitable task</returns>
    protected virtual Task OnKeyPressHandler( KeyboardEventArgs eventArgs )
    {
        return KeyPress.InvokeAsync( eventArgs );
    }

    /// <summary>
    /// Handler for <c>@onkeyup</c> event.
    /// </summary>
    /// <param name="eventArgs">Information about the keyboard up event.</param>
    /// <returns>Returns awaitable task</returns>
    protected virtual Task OnKeyUpHandler( KeyboardEventArgs eventArgs )
    {
        return KeyUp.InvokeAsync( eventArgs );
    }

    /// <summary>
    /// Handler for <c>@onblur</c> event.
    /// </summary>
    /// <param name="eventArgs">Information about the focus event.</param>
    /// <returns>Returns awaitable task</returns>
    protected virtual Task OnBlurHandler( FocusEventArgs eventArgs )
    {
        return Blur.InvokeAsync( eventArgs );
    }

    /// <summary>
    /// Handler for <c>@onfocus</c> event.
    /// </summary>
    /// <param name="eventArgs">Information about the focus event.</param>
    /// <returns>Returns awaitable task</returns>
    protected virtual Task OnFocusHandler( FocusEventArgs eventArgs )
    {
        return OnFocus.InvokeAsync( eventArgs );
    }

    /// <summary>
    /// Handler for <c>@onfocusin</c> event.
    /// </summary>
    /// <param name="eventArgs">Information about the focus event.</param>
    /// <returns>Returns awaitable task</returns>
    protected virtual Task OnFocusInHandler( FocusEventArgs eventArgs )
    {
        return FocusIn.InvokeAsync( eventArgs );
    }

    /// <summary>
    /// Handler for <c>@onfocusout</c> event.
    /// </summary>
    /// <param name="eventArgs">Information about the focus event.</param>
    /// <returns>Returns awaitable task</returns>
    protected virtual Task OnFocusOutHandler( FocusEventArgs eventArgs )
    {
        return FocusOut.InvokeAsync( eventArgs );
    }

    /// <summary>
    /// Forces the <see cref="Validation"/> (if any is used) to re-validate with the new custom or internal value.
    /// </summary>
    public Task Revalidate()
    {
        if ( ParentValidation is not null )
            return ParentValidation.NotifyInputChanged<TValue>( default );

        return Task.CompletedTask;
    }

    /// <summary>
    /// Handler for validation status change event.
    /// </summary>
    /// <param name="sender">Object that raised the event.</param>
    /// <param name="eventArgs">Information about the validation status.</param>
    protected virtual async void OnValidationStatusChanged( object sender, ValidationStatusChangedEventArgs eventArgs )
    {
        UpdateAriaAttributes();

        DirtyStyles();
        DirtyClasses();

        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Handler for validation message element id changes.
    /// </summary>
    private async void OnValidationMessageChanged()
    {
        UpdateAriaAttributes();

        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Handler for field help changes.
    /// </summary>
    private async void OnHelpTextChanged()
    {
        UpdateAriaAttributes();

        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Updates aria attributes based on validation and help text state.
    /// </summary>
    private void UpdateAriaAttributes()
    {
        UpdateAriaInvalidAttribute();
        UpdateAriaDescribedByAttribute();
    }

    private void UpdateAriaInvalidAttribute()
    {
        ariaInvalid = ParentValidation?.Status == ValidationStatus.Error ? "true" : null;
    }

    private void UpdateAriaDescribedByAttribute()
    {
        ariaDescribedBy = BuildAriaDescribedBy();
    }

    private string BuildAriaDescribedBy()
    {
        var helpTextId = ParentField?.HelpTextElementId;
        var errorTextId = ParentValidation?.Status == ValidationStatus.Error
            ? ParentValidation?.ValidationMessageElementId
            : null;

        if ( string.IsNullOrEmpty( helpTextId ) )
            return string.IsNullOrEmpty( errorTextId ) ? null : errorTextId;

        if ( string.IsNullOrEmpty( errorTextId ) )
            return helpTextId;

        return string.Equals( helpTextId, errorTextId, StringComparison.Ordinal )
            ? helpTextId
            : $"{helpTextId} {errorTextId}";
    }

    /// <summary>
    /// An event raised when theme settings changes.
    /// </summary>
    /// <param name="sender">An object that raised the event.</param>
    /// <param name="eventArgs"></param>
    private void OnThemeOptionsChanged( object sender, EventArgs eventArgs )
    {
        DirtyClasses();
        DirtyStyles();

        InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Gets the formatted value expression for the input component.
    /// </summary>
    /// <returns>Returns the formatted value expression for the input component</returns>
    protected virtual string GetFormatedValueExpression()
    {
        if ( ValueExpression is null )
            return null;

        return HtmlFieldPrefix is not null
            ? HtmlFieldPrefix.GetFieldName( ValueExpression )
            : ExpressionFormatter.FormatLambda( ValueExpression );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <inheritdoc/>
    public virtual object ValidationValue => CustomValidationValue is not null
        ? CustomValidationValue.Invoke()
        : Value;

    /// <summary>
    /// Returns true if input belong to a <see cref="FieldBody"/>.
    /// </summary>
    protected bool ParentIsFieldBody => ParentFieldBody is not null;

    /// <summary>
    /// Returns true if input belong to a <see cref="Addons"/>.
    /// </summary>
    protected bool ParentIsAddons => ParentAddons is not null;

    /// <summary>
    /// Returns the default value for the <typeparamref name="TValue"/> type.
    /// </summary>
    protected virtual TValue DefaultValue => default;

    /// <summary>
    /// Gets the value to be used for the input's "name" attribute.
    /// </summary>
    protected string NameAttributeValue
    {
        get
        {
            if ( shouldGenerateFieldNames && formattedValueExpression is null )
            {
                formattedValueExpression = GetFormatedValueExpression();
            }

            return formattedValueExpression;
        }
    }

    /// <summary>
    /// Gets or sets the current input value.
    /// </summary>
    protected TValue CurrentValue
    {
        get => Value;
        set
        {
            if ( !IsSameAsInternalValue( value ) )
            {
                Value = value;
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
    /// Gets the <see cref="ReadOnly"/> value represented as a string.
    /// </summary>
    protected string ReadOnlyAsString => ReadOnly ? "true" : "false";

    /// <summary>
    /// Gets the aria-invalid attribute value.
    /// </summary>
    protected string AriaInvalidAttribute
        => Attributes is not null && Attributes.ContainsKey( "aria-invalid" ) ? null : ariaInvalid;

    /// <summary>
    /// Gets the aria-describedby attribute value.
    /// </summary>
    protected string AriaDescribedByAttribute
        => Attributes is not null && Attributes.ContainsKey( "aria-describedby" ) ? null : ariaDescribedBy;

    /// <summary>
    /// Gets the size based on the theme settings.
    /// </summary>
    protected Size ThemeSize => Size.GetValueOrDefault( ParentAddons?.Size ?? ThemeOptions?.InputOptions?.Size ?? Blazorise.Size.Default );

    /// <summary>
    /// Gets the value indicating if the input is disabled.
    /// </summary>
    protected virtual bool IsDisabled => Disabled;

    /// <summary>
    /// Holds the field prefix for the input.
    /// </summary>
    [CascadingParameter] protected HtmlFieldPrefix HtmlFieldPrefix { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="IJSUtilitiesModule"/> instance.
    /// </summary>
    [Inject] public IJSUtilitiesModule JSUtilitiesModule { get; set; }

    /// <summary>
    /// Holds the information about the Blazorise global options.
    /// </summary>
    [Inject] protected BlazoriseOptions Options { get; set; }

    /// <summary>
    /// Gets or sets the value inside the input field.
    /// </summary>
    [Parameter] public virtual TValue Value { get; set; }

    /// <summary>
    /// Occurs after value has changed.
    /// </summary>
    [Parameter] public virtual EventCallback<TValue> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets an expression that identifies the input value.
    /// </summary>
    [Parameter] public virtual Expression<Func<TValue>> ValueExpression { get; set; }

    /// <summary>
    /// Sets the size of the input control.
    /// </summary>
    [Parameter]
    public Size? Size
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
            if ( disabled == value )
                return;

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
    /// Specifies the content to be rendered inside this <see cref="BaseInputComponent{TValue}"/>.
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
    /// Parent field container.
    /// </summary>
    [CascadingParameter] protected Field ParentField { get; set; }

    /// <summary>
    /// Parent field body.
    /// </summary>
    [CascadingParameter] protected FieldBody ParentFieldBody { get; set; }

    /// <summary>
    /// Gets or sets the reference to the parent addons.
    /// </summary>
    [CascadingParameter] protected Addons ParentAddons { get; set; }

    /// <summary>
    /// Parent focusable container.
    /// </summary>
    [CascadingParameter] protected IFocusableContainerComponent ParentFocusableContainer { get; set; }

    /// <summary>
    /// Cascaded theme settings.
    /// </summary>
    [CascadingParameter] protected Theme ThemeOptions { get; set; }

    #endregion
}

/// <summary>
/// Base component for all the input component types.
/// </summary>
/// <typeparam name="TValue">Editable value type.</typeparam>
public abstract class BaseInputComponent<TValue> : BaseInputComponent<TValue, ComponentClasses, ComponentStyles>
{
}