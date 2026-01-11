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
    #region Methods

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
    protected override Task OnChangeHandler( ChangeEventArgs e )
    {
        return CurrentValueHandler( e?.Value?.ToString() );
    }

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
    protected override Task OnBlurHandler( FocusEventArgs eventArgs )
    {
        // just call eventcallback without using debouncer in BaseTextInput
        return Blur.InvokeAsync( eventArgs );
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