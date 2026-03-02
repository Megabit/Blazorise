#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using Blazorise.Utilities;
#endregion

namespace Blazorise.DataGrid.Utils;

/// <summary>
/// Provides helper methods for normalizing DataGrid filter values.
/// </summary>
internal static class DataGridFilterUtils
{
    /// <summary>
    /// Coerces the supplied filter search value to the column underlying value type.
    /// </summary>
    /// <typeparam name="TItem">Type parameter for the model displayed in the <see cref="DataGrid{TItem}"/>.</typeparam>
    /// <param name="column">Column metadata used to determine the expected value type.</param>
    /// <param name="searchValue">Restored search value to coerce.</param>
    /// <returns>
    /// The coerced value for single filters, or a coerced array for range filters.
    /// If coercion fails, the original value is returned.
    /// </returns>
    public static object CoerceSearchValue<TItem>( DataGridColumn<TItem> column, object searchValue )
    {
        if ( searchValue is null )
        {
            return null;
        }

        if ( column.ColumnType != DataGridColumnType.Numeric && column.ColumnType != DataGridColumnType.Date )
        {
            return searchValue;
        }

        var columnValueType = column.GetValueType( default );

        if ( columnValueType is null || columnValueType == typeof( object ) )
        {
            return searchValue;
        }

        var isRangeValue = searchValue is object[]
            || string.Equals( searchValue.GetType().Name, "JArray", StringComparison.Ordinal );

        if ( isRangeValue && searchValue is IEnumerable enumerable )
        {
            var convertedValues = new List<object>();

            foreach ( var value in enumerable )
            {
                convertedValues.Add( CoerceValue( value, columnValueType ) );
            }

            return convertedValues.ToArray();
        }

        return CoerceValue( searchValue, columnValueType );
    }

    /// <summary>
    /// Coerces a single filter value to the target type.
    /// </summary>
    /// <param name="value">Value to coerce.</param>
    /// <param name="targetType">Target column value type.</param>
    /// <returns>
    /// The converted value when coercion succeeds; otherwise the original value.
    /// </returns>
    private static object CoerceValue( object value, Type targetType )
    {
        if ( value is null )
        {
            return null;
        }

        var normalizedType = Nullable.GetUnderlyingType( targetType ) ?? targetType;

        if ( normalizedType.IsInstanceOfType( value ) )
        {
            return value;
        }

        if ( Converters.TryChangeType( value, normalizedType, out var convertedValue ) )
        {
            return convertedValue;
        }

        var valueAsString = value.ToString();

        if ( string.IsNullOrWhiteSpace( valueAsString ) )
        {
            return null;
        }

        if ( Converters.TryChangeType( valueAsString, normalizedType, out convertedValue ) )
        {
            return convertedValue;
        }

        return value;
    }
}