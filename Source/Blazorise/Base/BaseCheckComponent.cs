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
/// Base class for all check-able components.
/// </summary>
/// <typeparam name="TValue">Checked value type.</typeparam>
public abstract class BaseCheckComponent<TValue> : BaseInputComponent<TValue>
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
    protected override Task OnInternalValueChanged( TValue value )
    {
        return ValueChanged.InvokeAsync( value );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the string value that represents the checked state.
    /// </summary>
    protected abstract string TrueValueName { get; }

    /// <summary>
    /// Group checkboxes or radios on the same horizontal row.
    /// </summary>
    [Parameter]
    public bool Inline
    {
        get => inline;
        set
        {
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
            cursor = value;

            DirtyClasses();
        }
    }

    #endregion
}