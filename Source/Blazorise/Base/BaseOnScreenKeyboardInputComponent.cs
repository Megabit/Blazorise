#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Base component for text-capable inputs that support the on-screen keyboard.
/// </summary>
/// <typeparam name="TValue">Editable value type.</typeparam>
/// <typeparam name="TClasses">Component-specific classes type.</typeparam>
/// <typeparam name="TStyles">Component-specific styles type.</typeparam>
public abstract class BaseOnScreenKeyboardInputComponent<TValue, TClasses, TStyles> : BaseInputComponent<TValue, TClasses, TStyles>
    where TClasses : ComponentClasses
    where TStyles : ComponentStyles
{
    #region Members

    /// <summary>
    /// Contains metadata about the parameter representing on-screen keyboard for the component.
    /// </summary>
    protected ComponentParameterInfo<bool> paramOnScreenKeyboard;

    /// <summary>
    /// Contains metadata about the parameter representing on-screen keyboard layout for the component.
    /// </summary>
    protected ComponentParameterInfo<OnScreenKeyboardLayout> paramOnScreenKeyboardLayout;

    /// <summary>
    /// Contains metadata about the parameter representing on-screen keyboard enter key behavior for the component.
    /// </summary>
    protected ComponentParameterInfo<OnScreenKeyboardEnterKeyBehavior> paramOnScreenKeyboardEnterKeyBehavior;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void CaptureParameters( ParameterView parameters )
    {
        base.CaptureParameters( parameters );

        parameters.TryGetParameter( OnScreenKeyboard, out paramOnScreenKeyboard );
        parameters.TryGetParameter( OnScreenKeyboardLayout, out paramOnScreenKeyboardLayout );
        parameters.TryGetParameter( OnScreenKeyboardEnterKeyBehavior, out paramOnScreenKeyboardEnterKeyBehavior );
    }

    /// <inheritdoc/>
    protected override void ReleaseResources()
    {
        _ = OnScreenKeyboardService?.Hide( ElementId );

        base.ReleaseResources();
    }

    /// <inheritdoc/>
    protected override async Task OnBlurHandler( FocusEventArgs eventArgs )
    {
        await base.OnBlurHandler( eventArgs );
        await HideOnScreenKeyboard();
    }

    /// <inheritdoc/>
    protected override async Task OnFocusInHandler( FocusEventArgs eventArgs )
    {
        await base.OnFocusInHandler( eventArgs );
        await ShowOnScreenKeyboard();
    }

    /// <summary>
    /// Shows the on-screen keyboard for this input.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual Task ShowOnScreenKeyboard()
    {
        var options = Options?.AccessibilityOptions?.OnScreenKeyboard;

        if ( !UseOnScreenKeyboard || options?.ShowOnFocus == false )
            return Task.CompletedTask;

        var context = new OnScreenKeyboardContext
        {
            ElementId = ElementId,
            ComponentType = GetType(),
            Layout = ResolvedOnScreenKeyboardLayout,
            GetValue = () => CurrentValueAsString,
            SetValue = SetOnScreenKeyboardValue,
            InsertText = InsertOnScreenKeyboardText,
            Backspace = BackspaceOnScreenKeyboard,
            Enter = OnScreenKeyboardEnter,
            Submit = SubmitOnScreenKeyboard,
            Disabled = IsDisabled,
            ReadOnly = ReadOnly,
        };

        if ( options?.LayoutSelector is not null )
        {
            context.Layout = options.LayoutSelector( context );
        }

        if ( options?.ShouldShow is not null && !options.ShouldShow( context ) )
            return OnScreenKeyboardService.Hide();

        return OnScreenKeyboardService.Show( context );
    }

    /// <summary>
    /// Hides the on-screen keyboard for this input.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual Task HideOnScreenKeyboard()
    {
        return Options?.AccessibilityOptions?.OnScreenKeyboard?.HideOnBlur == true && OnScreenKeyboardService?.ShouldIgnoreBlur != true
            ? OnScreenKeyboardService.Hide( ElementId )
            : Task.CompletedTask;
    }

    /// <summary>
    /// Sets the current value from the on-screen keyboard.
    /// </summary>
    /// <param name="value">New value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual Task SetOnScreenKeyboardValue( string value )
    {
        return InvokeAsync( async () =>
        {
            await CurrentValueHandler( value );
            StateHasChanged();
        } );
    }

    /// <summary>
    /// Inserts text from the on-screen keyboard at the current caret position.
    /// </summary>
    /// <param name="text">Text to insert.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual Task InsertOnScreenKeyboardText( string text )
    {
        if ( string.IsNullOrEmpty( text ) )
            return Task.CompletedTask;

        return InvokeAsync( async () =>
        {
            var value = CurrentValueAsString ?? string.Empty;
            var caret = await JSUtilitiesModule.GetCaret( ElementRef );

            caret = caret < 0
                ? value.Length
                : Math.Min( caret, value.Length );

            await CurrentValueHandler( value.Insert( caret, text ) );

            ExecuteAfterRender( () => JSUtilitiesModule.SetCaret( ElementRef, caret + text.Length ).AsTask() );

            StateHasChanged();
        } );
    }

    /// <summary>
    /// Removes text from the on-screen keyboard at the current caret position.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual Task BackspaceOnScreenKeyboard()
    {
        return InvokeAsync( async () =>
        {
            var value = CurrentValueAsString ?? string.Empty;
            var caret = await JSUtilitiesModule.GetCaret( ElementRef );

            caret = caret < 0
                ? value.Length
                : Math.Min( caret, value.Length );

            if ( caret == 0 || value.Length == 0 )
                return;

            await CurrentValueHandler( value.Remove( caret - 1, 1 ) );

            ExecuteAfterRender( () => JSUtilitiesModule.SetCaret( ElementRef, caret - 1 ).AsTask() );

            StateHasChanged();
        } );
    }

    /// <summary>
    /// Handles the on-screen keyboard enter key.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual async Task OnScreenKeyboardEnter()
    {
        var behavior = ResolvedOnScreenKeyboardEnterKeyBehavior;
        var shouldContinue = await DispatchOnScreenKeyboardEnterKeyDown();

        if ( behavior == OnScreenKeyboardEnterKeyBehavior.KeyDown || !shouldContinue )
            return;

        if ( behavior == OnScreenKeyboardEnterKeyBehavior.NewLine )
        {
            await InsertOnScreenKeyboardText( OnScreenKeyboardNewLineText );
            return;
        }

        if ( behavior == OnScreenKeyboardEnterKeyBehavior.Submit )
        {
            await SubmitOnScreenKeyboard();
            RestoreOnScreenKeyboardEnterFocus();

            return;
        }

        if ( behavior == OnScreenKeyboardEnterKeyBehavior.Hide && ShouldHideOnScreenKeyboardEnter )
        {
            await OnScreenKeyboardService.Hide( ElementId );
        }
    }

    /// <summary>
    /// Dispatches a bubbling Enter keydown event from the focused input component.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. Returns true if the event was not cancelled.</returns>
    protected virtual ValueTask<bool> DispatchOnScreenKeyboardEnterKeyDown()
    {
        return JSUtilitiesModule.DispatchKeyboardEvent( ElementRef, "keydown", "Enter", "Enter", 13 );
    }

    /// <summary>
    /// Restores focus to the input after a submit action so the virtual Enter key behaves like a physical Enter key.
    /// </summary>
    protected virtual void RestoreOnScreenKeyboardEnterFocus()
    {
        OnScreenKeyboardService.SuppressHideOnBlur();

        ExecuteAfterRender( async () =>
        {
            if ( OnScreenKeyboardService?.State.Visible == true
                && string.Equals( OnScreenKeyboardService.State.Context?.ElementId, ElementId, StringComparison.Ordinal ) )
            {
                OnScreenKeyboardService.SuppressHideOnBlur();
                await JSUtilitiesModule.Focus( ElementRef, ElementId, false );
            }
        } );
    }

    /// <summary>
    /// Submits the closest Blazorise validations or form from the on-screen keyboard.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual Task SubmitOnScreenKeyboard()
    {
        return ParentValidations is not null
            ? ParentValidations.RequestSubmit()
            : JSUtilitiesModule.SubmitClosestForm( ElementRef ).AsTask();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether this component supports the on-screen keyboard.
    /// </summary>
    protected virtual bool SupportsOnScreenKeyboard => true;

    /// <summary>
    /// Gets a value indicating whether the on-screen keyboard is enabled for this component.
    /// </summary>
    protected bool UseOnScreenKeyboard => SupportsOnScreenKeyboard
        && !IsDisabled
        && !ReadOnly
        && ( paramOnScreenKeyboard.Defined
            ? paramOnScreenKeyboard.Value
            : Options?.AccessibilityOptions?.OnScreenKeyboard?.Enabled == true );

    /// <summary>
    /// Gets the default on-screen keyboard layout for this component.
    /// </summary>
    protected virtual OnScreenKeyboardLayout DefaultOnScreenKeyboardLayout => OnScreenKeyboardLayout.Text;

    /// <summary>
    /// Gets the default on-screen keyboard enter key behavior for this component.
    /// </summary>
    protected virtual OnScreenKeyboardEnterKeyBehavior DefaultOnScreenKeyboardEnterKeyBehavior => ParentValidations is not null
        ? OnScreenKeyboardEnterKeyBehavior.Submit
        : OnScreenKeyboardEnterKeyBehavior.Hide;

    /// <summary>
    /// Gets the text inserted when the on-screen keyboard enter key behavior is <see cref="OnScreenKeyboardEnterKeyBehavior.NewLine"/>.
    /// </summary>
    protected virtual string OnScreenKeyboardNewLineText => Environment.NewLine;

    /// <summary>
    /// Gets the resolved on-screen keyboard layout for this component.
    /// </summary>
    protected OnScreenKeyboardLayout ResolvedOnScreenKeyboardLayout => paramOnScreenKeyboardLayout.Defined
        ? paramOnScreenKeyboardLayout.Value
        : ( Options?.AccessibilityOptions?.OnScreenKeyboard?.DefaultLayout ?? DefaultOnScreenKeyboardLayout );

    /// <summary>
    /// Gets the resolved on-screen keyboard enter key behavior for this component.
    /// </summary>
    protected OnScreenKeyboardEnterKeyBehavior ResolvedOnScreenKeyboardEnterKeyBehavior
    {
        get
        {
            var behavior = paramOnScreenKeyboardEnterKeyBehavior.Defined
                ? paramOnScreenKeyboardEnterKeyBehavior.Value
                : ( CascadedOnScreenKeyboardEnterKeyBehaviorOverride
                    ?? Options?.AccessibilityOptions?.OnScreenKeyboard?.EnterKeyBehavior
                    ?? OnScreenKeyboardEnterKeyBehavior.Default );

            return behavior == OnScreenKeyboardEnterKeyBehavior.Default
                ? DefaultOnScreenKeyboardEnterKeyBehavior
                : behavior;
        }
    }

    private bool ShouldHideOnScreenKeyboardEnter => IsOnScreenKeyboardHideEnterKeyBehaviorConfigured
        || Options?.AccessibilityOptions?.OnScreenKeyboard?.HideOnEnter == true;

    private bool IsOnScreenKeyboardHideEnterKeyBehaviorConfigured => ( paramOnScreenKeyboardEnterKeyBehavior.Defined && paramOnScreenKeyboardEnterKeyBehavior.Value == OnScreenKeyboardEnterKeyBehavior.Hide )
        || CascadedOnScreenKeyboardEnterKeyBehaviorOverride == OnScreenKeyboardEnterKeyBehavior.Hide
        || Options?.AccessibilityOptions?.OnScreenKeyboard?.EnterKeyBehavior == OnScreenKeyboardEnterKeyBehavior.Hide;

    /// <summary>
    /// Gets the cascaded on-screen keyboard enter key behavior.
    /// </summary>
    [CascadingParameter( Name = "OnScreenKeyboardEnterKeyBehaviorOverride" )] protected OnScreenKeyboardEnterKeyBehavior? CascadedOnScreenKeyboardEnterKeyBehaviorOverride { get; set; }

    /// <summary>
    /// Gets the parent validations component.
    /// </summary>
    [CascadingParameter] protected Validations ParentValidations { get; set; }

    /// <summary>
    /// Enables the on-screen keyboard for this input component. When not explicitly set, the global accessibility option is used.
    /// </summary>
    [Parameter] public bool OnScreenKeyboard { get; set; }

    /// <summary>
    /// Specifies the on-screen keyboard layout for this input component. When not explicitly set, the global accessibility option is used.
    /// </summary>
    [Parameter] public OnScreenKeyboardLayout OnScreenKeyboardLayout { get; set; }

    /// <summary>
    /// Specifies how the on-screen keyboard enter key should behave for this input component.
    /// </summary>
    [Parameter] public OnScreenKeyboardEnterKeyBehavior OnScreenKeyboardEnterKeyBehavior { get; set; }

    /// <summary>
    /// Gets the service that controls the on-screen keyboard.
    /// </summary>
    [Inject] protected IOnScreenKeyboardService OnScreenKeyboardService { get; set; }

    #endregion
}

/// <summary>
/// Base component for text-capable inputs that support the on-screen keyboard.
/// </summary>
/// <typeparam name="TValue">Editable value type.</typeparam>
public abstract class BaseOnScreenKeyboardInputComponent<TValue> : BaseOnScreenKeyboardInputComponent<TValue, ComponentClasses, ComponentStyles>
{
}