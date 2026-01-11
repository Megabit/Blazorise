#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// The browser built-in select dropdown.
/// </summary>
/// <typeparam name="TValue">The type of the <see cref="BaseInputComponent{TValue, TClasses, TStyles}.Value"/>.</typeparam>
public partial class Select<TValue> : BaseInputComponent<TValue, SelectClasses, SelectStyles>, ISelect
{
    #region Members

    private bool multiple;

    private bool loading;

    private readonly List<ISelectItem> selectItems = [];

    /// <summary>
    /// The internal value used by the select component.
    /// </summary>
    protected const string MULTIPLE_DELIMITER = "|~|";

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Select() );
        builder.Append( ClassProvider.SelectMultiple( Multiple ) );
        builder.Append( ClassProvider.SelectSize( ThemeSize ) );
        builder.Append( ClassProvider.SelectValidation( ParentValidation?.Status ?? ValidationStatus.None ) );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Handles the select onchange event.
    /// </summary>
    /// <param name="eventArgs">Supplies information about an change event that is being raised.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual Task OnChangeHandler( ChangeEventArgs eventArgs )
    {
        var value = Multiple && eventArgs?.Value is string[] multiValues
            ? string.Join( MULTIPLE_DELIMITER, multiValues )
            : eventArgs?.Value?.ToString();

        return CurrentValueHandler( value );
    }

    /// <inheritdoc/>
    protected override Task<ParseValue<TValue>> ParseValueFromStringAsync( string value )
    {
        if ( string.IsNullOrEmpty( value ) )
            return Task.FromResult( ParseValue<TValue>.Empty );

        if ( Multiple )
        {
            var readOnlyList = Converters.ConvertCsvToReadOnlyList<TValue>( value, MULTIPLE_DELIMITER );

            return Task.FromResult( new ParseValue<TValue>( true, readOnlyList, null ) );
        }
        else
        {
            if ( Converters.TryChangeType<TValue>( value, out var result ) )
            {
                return Task.FromResult( new ParseValue<TValue>( true, result, null ) );
            }
            else
            {
                return Task.FromResult( ParseValue<TValue>.Empty );
            }
        }
    }

    /// <inheritdoc/>
    protected override string FormatValueAsString( TValue value )
    {
        if ( value is null )
            return string.Empty;

        // Blazor expects the arrays as JSON formatted string
        if ( value is IEnumerable<TValue> values )
        {
            return Multiple
                ? JsonSerializer.Serialize( values.Select( x => x?.ToString() ) )
                : JsonSerializer.Serialize( values.FirstOrDefault()?.ToString() );
        }
        else if ( value is IEnumerable objects && CurrentValue is not string )
        {
            return Multiple
                ? JsonSerializer.Serialize( objects.Cast<object>().Select( x => x?.ToString() ) )
                : JsonSerializer.Serialize( objects.Cast<object>().FirstOrDefault()?.ToString() );
        }

        return value?.ToString();
    }

    /// <summary>
    /// Indicates if <see cref="Select{TValue}"/> contains the provided item value.
    /// </summary>
    /// <param name="value">Item value.</param>
    /// <returns>True if value is found.</returns>
    public bool ContainsValue( object value )
    {
        if ( CurrentValue is null )
            return false;

        if ( CurrentValue is IEnumerable<TValue> values )
        {
            return values.Any( x => x.IsEqual( value ) );
        }
        else if ( CurrentValue is IEnumerable objects && CurrentValue is not string )
        {
            return objects.Cast<object>().Any( x => x.IsEqual( value ) );
        }

        return CurrentValue.IsEqual( value );
    }

    /// <summary>
    /// Notifies the <see cref="ISelectItem"/> that it has been initialized.
    /// </summary>
    /// <param name="selectItem">The select item that has been initialized.</param>
    public void AddSelectItem( ISelectItem selectItem )
    {
        if ( selectItem is null )
            return;

        if ( !selectItems.Contains( selectItem ) )
            selectItems.Add( selectItem );
    }

    /// <summary>
    /// Notifies the <see cref="ISelectItem"/> that it has been removed.
    /// </summary>
    /// <param name="selectItem">The select item that has been removed.</param>
    public void RemoveSelectItem( ISelectItem selectItem )
    {
        if ( selectItem is null )
            return;

        if ( selectItems.Contains( selectItem ) )
            selectItems.Remove( selectItem );
    }

    /// <inheritdoc/>
    protected override bool IsSameAsInternalValue( TValue value )
    {
        if ( value is IEnumerable<TValue> values1 && Value is IEnumerable<TValue> values2 )
        {
            return values1.AreEqual( values2 );
        }
        else if ( value is IEnumerable objects1 && Value is IEnumerable objects2 )
        {
            return objects1.AreEqual( objects2 );
        }

        return value.IsEqual( Value );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the list of all select items inside of this select component.
    /// </summary>
    protected IEnumerable<ISelectItem> SelectItems => selectItems;

    /// <summary>
    /// Specifies that multiple items can be selected.
    /// </summary>
    [Parameter]
    public bool Multiple
    {
        get => multiple;
        set
        {
            multiple = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Specifies how many options should be shown at once.
    /// </summary>
    [Parameter] public int? MaxVisibleItems { get; set; }

    /// <summary>
    /// Gets or sets loading property.
    /// </summary>
    [Parameter]
    public bool Loading
    {
        get => loading;
        set
        {
            loading = value;
            Disabled = value;
        }
    }

    #endregion
}