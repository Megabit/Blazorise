#region Using directives
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// An editor that displays a date value and allows a user to edit the value.
/// </summary>
/// <typeparam name="TValue">Data-type to be binded by the <see cref="DateInput{TValue}"/> property.</typeparam>
public partial class DateInput<TValue> : BaseTextInput<TValue, DateInputClasses, DateInputStyles>
{
    #region Members

    private OnScreenKeyboardDateInputComposer onScreenKeyboardComposer;

    /// <summary>
    /// Stores the value reported by the native input, including an empty string while date segments are incomplete,
    /// so rendering does not overwrite the browser's in-progress entry with the formatted component value.
    /// A null value indicates that the input should display the formatted component value.
    /// </summary>
    private string nativeInputValue;

    /// <summary>
    /// Captured InputMode parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<DateInputMode> paramInputMode;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void CaptureParameters( ParameterView parameters )
    {
        base.CaptureParameters( parameters );

        parameters.TryGetParameter( InputMode, out paramInputMode );
    }

    /// <inheritdoc/>
    protected override Task OnAfterSetParametersAsync( ParameterView parameters )
    {
        if ( ( paramValue.Defined && paramValue.Changed ) || ( paramInputMode.Defined && paramInputMode.Changed ) )
        {
            nativeInputValue = null;
        }

        return base.OnAfterSetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DateInput( Plaintext ) );
        builder.Append( ClassProvider.DateInputSize( ThemeSize ) );
        builder.Append( ClassProvider.DateInputColor( Color ) );
        builder.Append( ClassProvider.DateInputValidation( ParentValidation?.Status ?? ValidationStatus.None ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        builder.Append( StyleProvider.DateInputColor( Color ) );

        base.BuildStyles( builder );
    }

    /// <inheritdoc/>
    protected override Task OnChangeHandler( ChangeEventArgs e )
    {
        // Native date inputs report an empty value while a segment is incomplete.
        // Keep that editing value so rendering does not reset the browser's segments.
        nativeInputValue = e?.Value?.ToString() ?? string.Empty;

        return CurrentValueHandler( nativeInputValue );
    }

    /// <summary>
    /// Handles changes to the native input's editing value.
    /// </summary>
    /// <param name="value">Value reported by the native input.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected Task OnNativeInputValueChanged( string value )
        => OnChangeHandler( new ChangeEventArgs { Value = value } );

    /// <inheritdoc/>
    protected override string FormatValueAsString( TValue value )
        => InputMode == DateInputMode.Week
            ? WeekDateFormat.FormatNativeValue( value )
            : Formaters.FormatDateValueAsString( value, DateFormat );

    /// <inheritdoc/>
    protected override Task<ParseValue<TValue>> ParseValueFromStringAsync( string value )
    {
        if ( Parsers.TryParseDate<TValue>( value, InputMode, out var result ) )
        {
            return Task.FromResult( new ParseValue<TValue>( true, result, null ) );
        }
        else
        {
            return Task.FromResult( new ParseValue<TValue>( false, default, null ) );
        }
    }

    /// <inheritdoc/>
    protected override Task OnKeyPressHandler( KeyboardEventArgs eventArgs )
    {
        // just call eventcallback without using debouncer in BaseTextInput
        return KeyPress.InvokeAsync( eventArgs );
    }

    /// <inheritdoc/>
    public override Task ShowOnScreenKeyboard( bool focus = true )
    {
        onScreenKeyboardComposer = new( InputMode, OnScreenKeyboardRequiresSeconds );
        onScreenKeyboardComposer.Reset( CurrentValueAsString );

        return base.ShowOnScreenKeyboard( focus );
    }

    /// <inheritdoc/>
    protected override Task SetOnScreenKeyboardValue( string value )
    {
        return UpdateOnScreenKeyboardDateValue( OnScreenKeyboardComposer.SetValue( value, CanParseOnScreenKeyboardDateValue ) );
    }

    /// <inheritdoc/>
    protected override Task InsertOnScreenKeyboardText( string text )
    {
        return UpdateOnScreenKeyboardDateValue( OnScreenKeyboardComposer.InsertText( text, CanParseOnScreenKeyboardDateValue ) );
    }

    /// <inheritdoc/>
    protected override Task BackspaceOnScreenKeyboard()
    {
        return UpdateOnScreenKeyboardDateValue( OnScreenKeyboardComposer.Backspace( CanParseOnScreenKeyboardDateValue ) );
    }

    /// <inheritdoc/>
    protected override async Task OnScreenKeyboardEnter()
    {
        await UpdateOnScreenKeyboardDateValue( OnScreenKeyboardComposer.Complete( CanParseOnScreenKeyboardDateValue ) );

        await base.OnScreenKeyboardEnter();
    }

    /// <inheritdoc/>
    protected override string GetOnScreenKeyboardPreviewValue()
    {
        return OnScreenKeyboardComposer.PreviewValue;
    }

    private Task UpdateOnScreenKeyboardDateValue( OnScreenKeyboardInputComposition composition )
    {
        if ( composition.CanCommit )
        {
            nativeInputValue = composition.Value ?? string.Empty;
        }

        return UpdateOnScreenKeyboardEditingValue( composition.Value, composition.CanCommit, composition.CanCommit );
    }

    private bool CanParseOnScreenKeyboardDateValue( string value )
    {
        return Parsers.TryParseDate( value, InputMode, out TValue _ );
    }

    /// <summary>
    /// Show a browser picker for the date input.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task ShowPicker()
    {
        return JSUtilitiesModule.ShowPicker( ElementRef, ElementId ).AsTask();
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <inheritdoc/>
    protected override OnScreenKeyboardLayout DefaultOnScreenKeyboardLayout => OnScreenKeyboardLayout.Numeric;

    /// <inheritdoc/>
    protected override OnScreenKeyboardInputType OnScreenKeyboardInputType => InputMode == DateInputMode.DateTime
        ? OnScreenKeyboardInputType.Date | OnScreenKeyboardInputType.Time
        : OnScreenKeyboardInputType.Date;

    private OnScreenKeyboardDateInputComposer OnScreenKeyboardComposer => onScreenKeyboardComposer ??= new( InputMode, OnScreenKeyboardRequiresSeconds );

    private bool OnScreenKeyboardRequiresSeconds => InputMode == DateInputMode.DateTime && Step < 60;

    /// <summary>
    /// Gets the string representation of the input mode.
    /// </summary>
    protected string Mode => InputMode.ToDateInputMode();

    /// <summary>
    /// Gets the native editing value without replacing incomplete input with the formatted date.
    /// </summary>
    protected string NativeInputValue => nativeInputValue ?? CurrentValueAsString ?? string.Empty;

    /// <summary>
    /// Gets the date format based on the current <see cref="InputMode"/> settings.
    /// </summary>
    protected string DateFormat => Parsers.GetInternalDateFormat( InputMode );

    /// <summary>
    /// Gets the minimum value rendered by the input.
    /// </summary>
    protected string InputMin => InputMode == DateInputMode.Week
        ? WeekDateFormat.FormatNativeValue( Min )
        : Min?.ToString( DateFormat );

    /// <summary>
    /// Gets the maximum value rendered by the input.
    /// </summary>
    protected string InputMax => InputMode == DateInputMode.Week
        ? WeekDateFormat.FormatNativeValue( Max )
        : Max?.ToString( DateFormat );

    /// <summary>
    /// Hints at the type of data that might be entered by the user while editing the element or its contents.
    /// </summary>
    [Parameter] public DateInputMode InputMode { get; set; } = DateInputMode.Date;

    /// <summary>
    /// The earliest date to accept.
    /// </summary>
    [Parameter] public DateTimeOffset? Min { get; set; }

    /// <summary>
    /// The latest date to accept.
    /// </summary>
    [Parameter] public DateTimeOffset? Max { get; set; }

    /// <summary>
    /// The step attribute specifies the legal day intervals to choose from when the user opens the calendar in a date field.
    ///
    /// For example, if step = "2", you can only select every second day in the calendar.
    /// </summary>
    [Parameter] public int Step { get; set; } = 1;

    #endregion
}