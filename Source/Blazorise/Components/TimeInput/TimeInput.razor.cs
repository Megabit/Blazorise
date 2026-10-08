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
/// An editor that displays a time value and allows a user to edit the value.
/// </summary>
/// <typeparam name="TValue">Data-type to be binded by the <see cref="TimeInput{TValue}"/> property.</typeparam>
public partial class TimeInput<TValue> : BaseTextInput<TValue, TimeInputClasses, TimeInputStyles>
{
    #region Members

    private OnScreenKeyboardTimeInputComposer onScreenKeyboardComposer;

    /// <summary>
    /// Stores the value reported by the native input, including an empty string while time segments are incomplete,
    /// so rendering does not overwrite the browser's in-progress entry with the formatted component value.
    /// A null value indicates that the input should display the formatted component value.
    /// </summary>
    private string nativeInputValue;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnAfterSetParametersAsync( ParameterView parameters )
    {
        if ( paramValue.Defined && paramValue.Changed )
        {
            nativeInputValue = null;
        }

        return base.OnAfterSetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.TimeInput( Plaintext ) );
        builder.Append( ClassProvider.TimeInputSize( ThemeSize ) );
        builder.Append( ClassProvider.TimeInputColor( Color ) );
        builder.Append( ClassProvider.TimeInputValidation( ParentValidation?.Status ?? ValidationStatus.None ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        builder.Append( StyleProvider.TimeInputColor( Color ) );

        base.BuildStyles( builder );
    }

    /// <inheritdoc/>
    protected override Task OnChangeHandler( ChangeEventArgs e )
    {
        // Native time inputs report an empty value while a segment is incomplete.
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
    {
        return value switch
        {
            null => null,
            TimeSpan timeSpan => timeSpan.ToString( Parsers.InternalTimeFormat.ToLowerInvariant() ),
            TimeOnly timeOnly => timeOnly.ToString( Parsers.InternalTimeFormat ),
            DateTime datetime => datetime.ToString( Parsers.InternalTimeFormat ),
            _ => throw new InvalidOperationException( $"Unsupported type {value.GetType()}" ),
        };
    }

    /// <inheritdoc/>
    protected override Task<ParseValue<TValue>> ParseValueFromStringAsync( string value )
    {
        if ( Parsers.TryParseTime<TValue>( value, out var result ) )
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
        onScreenKeyboardComposer = new( OnScreenKeyboardRequiresSeconds );
        onScreenKeyboardComposer.Reset( CurrentValueAsString );

        return base.ShowOnScreenKeyboard( focus );
    }

    /// <inheritdoc/>
    protected override Task SetOnScreenKeyboardValue( string value )
    {
        return UpdateOnScreenKeyboardTimeValue( OnScreenKeyboardComposer.SetValue( value, CanParseOnScreenKeyboardTimeValue ) );
    }

    /// <inheritdoc/>
    protected override Task InsertOnScreenKeyboardText( string text )
    {
        return UpdateOnScreenKeyboardTimeValue( OnScreenKeyboardComposer.InsertText( text, CanParseOnScreenKeyboardTimeValue ) );
    }

    /// <inheritdoc/>
    protected override Task BackspaceOnScreenKeyboard()
    {
        return UpdateOnScreenKeyboardTimeValue( OnScreenKeyboardComposer.Backspace( CanParseOnScreenKeyboardTimeValue ) );
    }

    /// <inheritdoc/>
    protected override async Task OnScreenKeyboardEnter()
    {
        await UpdateOnScreenKeyboardTimeValue( OnScreenKeyboardComposer.Complete( CanParseOnScreenKeyboardTimeValue ) );

        await base.OnScreenKeyboardEnter();
    }

    /// <inheritdoc/>
    protected override string GetOnScreenKeyboardPreviewValue()
    {
        return OnScreenKeyboardComposer.PreviewValue;
    }

    private Task UpdateOnScreenKeyboardTimeValue( OnScreenKeyboardInputComposition composition )
    {
        if ( composition.CanCommit )
        {
            nativeInputValue = composition.Value ?? string.Empty;
        }

        return UpdateOnScreenKeyboardEditingValue( composition.Value, composition.CanCommit, composition.CanCommit );
    }

    private bool CanParseOnScreenKeyboardTimeValue( string value )
    {
        TValue parsedValue;

        return ( value.Length == 5 || value.Length == 8 )
            && Parsers.TryParseTime<TValue>( value, out parsedValue );
    }

    /// <summary>
    /// Show a browser picker for the time input.
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
    protected override OnScreenKeyboardInputType OnScreenKeyboardInputType => OnScreenKeyboardInputType.Time;

    private OnScreenKeyboardTimeInputComposer OnScreenKeyboardComposer => onScreenKeyboardComposer ??= new( OnScreenKeyboardRequiresSeconds );

    private bool OnScreenKeyboardRequiresSeconds => Step.HasValue && Step.Value < 60;

    /// <summary>
    /// Gets the native input type as an expression so binding uses the native string value instead of typed time conversion.
    /// </summary>
    protected string Mode => "time";

    /// <summary>
    /// Gets the native editing value without replacing incomplete input with the formatted time.
    /// </summary>
    protected string NativeInputValue => nativeInputValue ?? CurrentValueAsString ?? string.Empty;

    /// <summary>
    /// The earliest time to accept.
    /// </summary>
    [Parameter] public TimeSpan? Min { get; set; }

    /// <summary>
    /// The latest time to accept.
    /// </summary>
    [Parameter] public TimeSpan? Max { get; set; }

    /// <summary>
    /// The step attribute specifies the legal number intervals for seconds or milliseconds in a time field (does not apply for hours or minutes).
    /// 
    /// Example: if step="2", legal numbers could be 0, 2, 4, etc.
    /// </summary>
    /// <remarks>
    /// The step attribute is often used together with the max and min attributes to create a range of legal values.
    /// </remarks>
    [Parameter] public int? Step { get; set; }

    #endregion
}