#region Using directives
using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Base class for all radio based components.
/// </summary>
/// <typeparam name="TValue">Radio value type.</typeparam>
public abstract class BaseRadioComponent<TValue> : BaseInputComponent<TValue>
{
    #region Members

    private bool inline;

    private Cursor cursor;

    #endregion

    #region Methods

    /// <summary>
    /// Handles the check input onchange event.
    /// </summary>
    /// <param name="eventArgs">Supplies information about an change event that is being raised.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual Task OnChangeHandler( ChangeEventArgs eventArgs )
    {
        return CurrentValueHandler( eventArgs?.Value?.ToString() );
    }

    /// <inheritdoc/>
    protected override Task<ParseValue<TValue>> ParseValueFromStringAsync( string value )
    {
        var parsedValue = ( value?.ToLowerInvariant() == TrueValueName?.ToLowerInvariant() ).ToString();

        if ( Converters.TryChangeType<TValue>( parsedValue, out var result, CultureInfo.InvariantCulture ) )
        {
            return Task.FromResult( new ParseValue<TValue>( true, result, null ) );
        }
        else
        {
            return Task.FromResult( ParseValue<TValue>.Empty );
        }
    }

    /// <inheritdoc/>
    protected override async Task OnInternalValueChanged( TValue value )
    {
        Checked = value?.ToString() == bool.TrueString;
        await CheckedChanged.InvokeAsync( Checked );

        await ValueChanged.InvokeAsync( value );
    }

    /// <inheritdoc/>
    protected override string GetFormatedValueExpression()
    {
        if ( CheckedExpression is null )
            return null;

        return HtmlFieldPrefix is not null
            ? HtmlFieldPrefix.GetFieldName( CheckedExpression )
            : ExpressionFormatter.FormatLambda( CheckedExpression );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the string value that represents the checked state.
    /// </summary>
    protected virtual string TrueValueName => Value.ToString();

    /// <summary>
    /// Gets or sets the checked flag.
    /// </summary>
    [Obsolete( "The 'Checked' property is obsolete and will be removed in future versions. Use 'Value' instead." )]
    [Parameter] public bool Checked { get; set; }

    /// <summary>
    /// Occurs when the check state is changed.
    /// </summary>
    [Obsolete( "The 'CheckedChanged' property is obsolete and will be removed in future versions. Use 'ValueChanged' instead." )]
    [Parameter] public EventCallback<bool> CheckedChanged { get; set; }

    /// <summary>
    /// Gets or sets an expression that identifies the checked value.
    /// </summary>
    [Obsolete( "The 'CheckedExpression' property is obsolete and will be removed in future versions. Use 'ValueExpression' instead." )]
    [Parameter] public Expression<Func<bool>> CheckedExpression { get; set; }

    /// <summary>
    /// Group checkboxes or radios on the same horizontal row.
    /// </summary>
    [Parameter]
    public bool Inline
    {
        get => inline;
        set
        {
            if ( inline == value )
                return;

            inline = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Defines the mouse cursor based on the behaviour by the current css framework.
    /// </summary>
    [Parameter]
    public Cursor Cursor
    {
        get => cursor;
        set
        {
            if ( cursor == value )
                return;

            cursor = value;

            DirtyClasses();
        }
    }

    #endregion
}