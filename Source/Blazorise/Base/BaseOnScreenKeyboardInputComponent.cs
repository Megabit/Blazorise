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

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void CaptureParameters( ParameterView parameters )
    {
        base.CaptureParameters( parameters );

        parameters.TryGetParameter( OnScreenKeyboard, out paramOnScreenKeyboard );
        parameters.TryGetParameter( OnScreenKeyboardLayout, out paramOnScreenKeyboardLayout );
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
        return Options?.AccessibilityOptions?.OnScreenKeyboard?.HideOnBlur == true
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
    protected virtual Task OnScreenKeyboardEnter()
    {
        return Options?.AccessibilityOptions?.OnScreenKeyboard?.HideOnEnter == true
            ? OnScreenKeyboardService.Hide( ElementId )
            : Task.CompletedTask;
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
    /// Gets the resolved on-screen keyboard layout for this component.
    /// </summary>
    protected OnScreenKeyboardLayout ResolvedOnScreenKeyboardLayout => paramOnScreenKeyboardLayout.Defined
        ? paramOnScreenKeyboardLayout.Value
        : ( Options?.AccessibilityOptions?.OnScreenKeyboard?.DefaultLayout ?? DefaultOnScreenKeyboardLayout );

    /// <summary>
    /// Enables the on-screen keyboard for this input component. When not explicitly set, the global accessibility option is used.
    /// </summary>
    [Parameter] public bool OnScreenKeyboard { get; set; }

    /// <summary>
    /// Specifies the on-screen keyboard layout for this input component. When not explicitly set, the global accessibility option is used.
    /// </summary>
    [Parameter] public OnScreenKeyboardLayout OnScreenKeyboardLayout { get; set; }

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