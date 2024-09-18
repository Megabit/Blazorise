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
/// <typeparam name="TValue">Data-type to be binded by the <see cref="DateEdit{TValue}"/> property.</typeparam>
public partial class DateEdit<TValue> : BaseTextInput<TValue>
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
        builder.Append( ClassProvider.DateEdit( Plaintext ) );
        builder.Append( ClassProvider.DateEditSize( ThemeSize ) );
        builder.Append( ClassProvider.DateEditColor( Color ) );
        builder.Append( ClassProvider.DateEditValidation( ParentValidation?.Status ?? ValidationStatus.None ) );

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
        => Formaters.FormatDateValueAsString( value, DateFormat );

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
    protected override Task OnBlurHandler( FocusEventArgs eventArgs )
    {
        // just call eventcallback without using debouncer in BaseTextInput
        return Blur.InvokeAsync( eventArgs );
    }

    /// <summary>
    /// Show a browser picker for the date input.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task ShowPicker()
    {
        return JSUtilitiesModule.ShowPicker( ElementRef, ElementId ).AsTask();
    }

    /// <inheritdoc/>
    protected override string GetFormatedValueExpression()
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

    /// <summary>
    /// Gets the string representation of the input mode.
    /// </summary>
    protected string Mode => InputMode.ToDateInputMode();

    /// <summary>
    /// Gets the date format based on the current <see cref="InputMode"/> settings.
    /// </summary>
    protected string DateFormat => Parsers.GetInternalDateFormat( InputMode );

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