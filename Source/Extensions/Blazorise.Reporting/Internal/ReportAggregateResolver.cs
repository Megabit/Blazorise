#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportAggregateResolver
{
    #region Methods

    internal static IReadOnlyList<ReportAggregateFunction> GetSupportedFunctions( ReportDefinition definition, object data, ReportSectionDefinition section, ReportElementDefinition element )
    {
        if ( section?.Type != ReportSectionType.Detail
            || element is not ReportFieldElementDefinition fieldElement
            || string.IsNullOrWhiteSpace( fieldElement.Field ) )
        {
            return [];
        }

        var values = ResolveFieldValues( definition, data, section.DataSource ?? fieldElement.DataSource, fieldElement.Field, null )
            .Where( value => value is not null )
            .ToList();

        return GetSupportedFunctions( values );
    }

    internal static IReadOnlyList<ReportAggregateFunction> GetSupportedFunctions( ReportDefinition definition, object data, string dataSource, string field )
    {
        return GetSupportedFunctions( definition, data, dataSource, field, null );
    }

    internal static IReadOnlyList<ReportAggregateFunction> GetSupportedFunctions( ReportDefinition definition, object data, string dataSource, string field, Type dataType )
    {
        if ( string.IsNullOrWhiteSpace( field ) )
            return [];

        var values = ResolveFieldValues( definition, data, dataSource, field, null )
            .Where( value => value is not null )
            .ToList();

        return values.Count > 0
            ? GetSupportedFunctions( values )
            : GetSupportedFunctions( dataType );
    }

    internal static object ResolveAggregateValue( ReportDefinition definition, object data, object item, ReportElementDefinition element )
    {
        if ( element is not ReportFieldElementDefinition fieldElement || fieldElement.Aggregate is null || string.IsNullOrWhiteSpace( fieldElement.Field ) )
            return null;

        return ResolveAggregateValue( definition, data, item, fieldElement.Aggregate.Function, fieldElement.DataSource, fieldElement.Field );
    }

    internal static object ResolveAggregateValue( ReportDefinition definition, object data, object item, ReportAggregateFunction function, string dataSource, string field )
    {
        if ( string.IsNullOrWhiteSpace( field ) )
        {
            return function == ReportAggregateFunction.Count
                ? ReportDataResolver.ResolveItems( definition, data, dataSource, item ).Count()
                : null;
        }

        var values = ResolveFieldValues( definition, data, dataSource, field, item )
            .Where( value => value is not null )
            .ToList();

        return function switch
        {
            ReportAggregateFunction.Count => values.Count,
            ReportAggregateFunction.Sum => Sum( values ),
            ReportAggregateFunction.Average => Average( values ),
            ReportAggregateFunction.Minimum => Minimum( values ),
            ReportAggregateFunction.Maximum => Maximum( values ),
            _ => null,
        };
    }

    internal static string GetFunctionDisplayName( ReportAggregateFunction function )
    {
        return function switch
        {
            ReportAggregateFunction.Count => "Count",
            ReportAggregateFunction.Sum => "Sum",
            ReportAggregateFunction.Average => "Average",
            ReportAggregateFunction.Minimum => "Minimum",
            ReportAggregateFunction.Maximum => "Maximum",
            _ => function.ToString(),
        };
    }

    private static IReadOnlyList<ReportAggregateFunction> GetSupportedFunctions( IReadOnlyList<object> values )
    {
        var supportedFunctions = new List<ReportAggregateFunction>
        {
            ReportAggregateFunction.Count,
        };

        if ( values.Count == 0 )
            return supportedFunctions;

        if ( values.All( IsNumericValue ) )
        {
            supportedFunctions.Add( ReportAggregateFunction.Sum );
            supportedFunctions.Add( ReportAggregateFunction.Average );
        }

        if ( HasCompatibleComparableValues( values ) )
        {
            supportedFunctions.Add( ReportAggregateFunction.Minimum );
            supportedFunctions.Add( ReportAggregateFunction.Maximum );
        }

        return supportedFunctions;
    }

    private static IReadOnlyList<ReportAggregateFunction> GetSupportedFunctions( Type dataType )
    {
        var supportedFunctions = new List<ReportAggregateFunction>
        {
            ReportAggregateFunction.Count,
        };

        if ( dataType is null )
            return supportedFunctions;

        if ( IsNumericType( dataType ) )
        {
            supportedFunctions.Add( ReportAggregateFunction.Sum );
            supportedFunctions.Add( ReportAggregateFunction.Average );
        }

        if ( IsNumericType( dataType ) || IsComparableType( dataType ) )
        {
            supportedFunctions.Add( ReportAggregateFunction.Minimum );
            supportedFunctions.Add( ReportAggregateFunction.Maximum );
        }

        return supportedFunctions;
    }

    private static decimal? Average( IReadOnlyList<object> values )
    {
        var numbers = values.Select( ConvertToDecimal ).Where( value => value is not null ).Select( value => value.Value ).ToList();

        return numbers.Count == 0 ? null : numbers.Sum() / numbers.Count;
    }

    private static int CompareValues( object value, object otherValue )
    {
        if ( value is null && otherValue is null )
            return 0;

        if ( value is null )
            return -1;

        if ( otherValue is null )
            return 1;

        if ( IsNumericValue( value ) && IsNumericValue( otherValue ) )
            return ConvertToDecimal( value ).GetValueOrDefault().CompareTo( ConvertToDecimal( otherValue ).GetValueOrDefault() );

        return value is IComparable comparable
            ? comparable.CompareTo( otherValue )
            : string.Compare( Convert.ToString( value, CultureInfo.CurrentCulture ), Convert.ToString( otherValue, CultureInfo.CurrentCulture ), StringComparison.CurrentCulture );
    }

    private static decimal? ConvertToDecimal( object value )
    {
        try
        {
            return value is null ? null : Convert.ToDecimal( value, CultureInfo.CurrentCulture );
        }
        catch
        {
            return null;
        }
    }

    private static bool IsComparableValue( object value )
    {
        return value is IComparable;
    }

    private static bool IsComparableType( Type type )
    {
        type = Nullable.GetUnderlyingType( type ) ?? type;

        return typeof( IComparable ).IsAssignableFrom( type );
    }

    private static bool HasCompatibleComparableValues( IReadOnlyList<object> values )
    {
        if ( values.Any( value => !IsComparableValue( value ) ) )
            return false;

        if ( values.All( IsNumericValue ) )
            return true;

        return values.Select( value => Nullable.GetUnderlyingType( value.GetType() ) ?? value.GetType() ).Distinct().Count() == 1;
    }

    private static bool IsNumericValue( object value )
    {
        var type = Nullable.GetUnderlyingType( value?.GetType() ) ?? value?.GetType();

        return IsNumericType( type );
    }

    private static bool IsNumericType( Type type )
    {
        return type == typeof( byte )
            || type == typeof( sbyte )
            || type == typeof( short )
            || type == typeof( ushort )
            || type == typeof( int )
            || type == typeof( uint )
            || type == typeof( long )
            || type == typeof( ulong )
            || type == typeof( float )
            || type == typeof( double )
            || type == typeof( decimal );
    }

    private static object Maximum( IReadOnlyList<object> values )
    {
        return values.Count == 0 ? null : values.Aggregate( ( current, value ) => CompareValues( current, value ) >= 0 ? current : value );
    }

    private static object Minimum( IReadOnlyList<object> values )
    {
        return values.Count == 0 ? null : values.Aggregate( ( current, value ) => CompareValues( current, value ) <= 0 ? current : value );
    }

    private static IEnumerable<object> ResolveFieldValues( ReportDefinition definition, object data, string dataSource, string field, object item )
    {
        foreach ( var sourceItem in ReportDataResolver.ResolveItems( definition, data, dataSource, item ) )
        {
            var value = ReportExpressionResolver.ResolveValue( definition, data, sourceItem, field );

            if ( value is IEnumerable enumerable and not string and not IDictionary )
            {
                foreach ( var objectValue in enumerable )
                {
                    yield return objectValue;
                }
            }
            else
            {
                yield return value;
            }
        }
    }

    private static decimal? Sum( IReadOnlyList<object> values )
    {
        var numbers = values.Select( ConvertToDecimal ).Where( value => value is not null ).Select( value => value.Value ).ToList();

        return numbers.Count == 0 ? null : numbers.Sum();
    }

    #endregion
}