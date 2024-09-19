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
/// <typeparam name="TValue">Data-type to be binded by the <see cref="TimeEdit{TValue}"/> property.</typeparam>
public partial class TimeEdit<TValue> : BaseTextInput<TValue>
{
    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        if ( Rendered )
        {
            if ( parameters.TryGetValue<TValue>( nameof( Value ), out var paramValue ) && !paramValue.IsEqual( Value ) )
            {
                ExecuteAfterRender( Revalidate );
            }
        }

        await base.SetParametersAsync( parameters );

        if ( ParentValidation is not null )
        {
            if ( parameters.TryGetValue<Expression<Func<TValue>>>( nameof( ValueExpression ), out var expression ) )
                await ParentValidation.InitializeInputExpression( expression );

            if ( parameters.TryGetValue<string>( nameof( Pattern ), out var paramPattern ) )
            {
                // make sure we get the newest value
                var newValue = parameters.TryGetValue<TValue>( nameof( Value ), out var paramValue )
                    ? paramValue
                    : Value;

                await ParentValidation.InitializeInputPattern( paramPattern, newValue );
            }

            await InitializeValidation();
        }
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.TimeEdit( Plaintext ) );
        builder.Append( ClassProvider.TimeEditSize( ThemeSize ) );
        builder.Append( ClassProvider.TimeEditColor( Color ) );
        builder.Append( ClassProvider.TimeEditValidation( ParentValidation?.Status ?? ValidationStatus.None ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override Task OnChangeHandler( ChangeEventArgs e )
    {
        return CurrentValueHandler( e?.Value?.ToString() );
    }

    /// <inheritdoc/>
    protected override Task OnInternalValueChanged( TValue value )
    {
        return ValueChanged.InvokeAsync( value );
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