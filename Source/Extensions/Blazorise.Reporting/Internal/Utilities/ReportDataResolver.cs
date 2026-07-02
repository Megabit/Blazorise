#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportDataResolver
{
    #region Methods

    internal static IEnumerable<object> ResolveItems( ReportDefinition definition, object defaultData, string dataSource = null, object currentItem = null )
    {
        var source = ResolveDataSourceValue( definition, defaultData, dataSource, currentItem );

        if ( source is IEnumerable enumerable and not string )
        {
            foreach ( var item in enumerable )
            {
                yield return item;
            }

            yield break;
        }

        if ( source is not null )
            yield return source;
    }

    internal static object ResolveDataSourceValue( ReportDefinition definition, object defaultData, string dataSource = null, object currentItem = null )
    {
        if ( string.IsNullOrWhiteSpace( dataSource ) )
            return currentItem ?? definition?.DataSources.FirstOrDefault()?.Data ?? defaultData;

        var trimmedDataSource = dataSource.Trim();
        var namedDataSource = definition?.DataSources.FirstOrDefault( x =>
            string.Equals( x.Name, trimmedDataSource, StringComparison.OrdinalIgnoreCase ) );

        if ( namedDataSource is not null )
            return namedDataSource.Data;

        var pathSeparatorIndex = trimmedDataSource.IndexOf( ".", StringComparison.Ordinal );

        if ( pathSeparatorIndex > 0 )
        {
            var dataSourceName = trimmedDataSource[..pathSeparatorIndex];
            var dataSourcePath = trimmedDataSource[( pathSeparatorIndex + 1 )..];
            namedDataSource = definition?.DataSources.FirstOrDefault( x =>
                string.Equals( x.Name, dataSourceName, StringComparison.OrdinalIgnoreCase ) );

            if ( namedDataSource is not null )
                return ResolvePathValue( namedDataSource.Data, dataSourcePath );
        }

        if ( currentItem is not null )
        {
            if ( TryResolvePathValue( currentItem, trimmedDataSource, out var relativeValue ) )
                return relativeValue;
        }

        return ResolvePathValue( definition?.DataSources.FirstOrDefault()?.Data ?? defaultData, trimmedDataSource );
    }

    internal static object ResolvePathValue( object item, string path )
    {
        if ( item is null || string.IsNullOrWhiteSpace( path ) )
            return null;

        var current = item;

        foreach ( var segment in path.Split( '.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries ) )
        {
            current = ResolvePathSegment( current, segment );

            if ( current is null )
                return null;
        }

        return current;
    }

    internal static bool TryResolvePathValue( object item, string path, out object value )
    {
        value = null;

        if ( item is null || string.IsNullOrWhiteSpace( path ) )
            return false;

        var current = item;
        var segments = path.Split( '.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries );

        for ( var i = 0; i < segments.Length; i++ )
        {
            if ( !TryResolvePathSegment( current, segments[i], out current ) )
                return false;

            if ( current is null && i < segments.Length - 1 )
                return false;
        }

        value = current;

        return true;
    }

    internal static string FormatValue( object value, string format )
    {
        if ( value is null )
            return string.Empty;

        if ( string.IsNullOrWhiteSpace( format ) )
            return Convert.ToString( value, CultureInfo.CurrentCulture );

        return value is IFormattable formattable
            ? formattable.ToString( format, CultureInfo.CurrentCulture )
            : Convert.ToString( value, CultureInfo.CurrentCulture );
    }

    private static object ResolvePathSegment( object item, string segment )
    {
        if ( item is null || string.IsNullOrWhiteSpace( segment ) )
            return null;

        if ( item is IEnumerable enumerable and not string and not IDictionary )
        {
            var values = new List<object>();

            foreach ( var childItem in enumerable )
            {
                var value = ResolvePathSegment( childItem, segment );

                if ( value is IEnumerable childEnumerable and not string and not IDictionary )
                    values.AddRange( childEnumerable.Cast<object>() );
                else if ( value is not null )
                    values.Add( value );
            }

            return values;
        }

        if ( item is IDictionary<string, object> dictionary )
        {
            var key = dictionary.Keys.FirstOrDefault( x => string.Equals( x, segment, StringComparison.OrdinalIgnoreCase ) );

            return key is not null && dictionary.TryGetValue( key, out var dictionaryValue )
                ? dictionaryValue
                : null;
        }

        if ( item is IDictionary nonGenericDictionary )
        {
            foreach ( var key in nonGenericDictionary.Keys )
            {
                if ( string.Equals( Convert.ToString( key, CultureInfo.InvariantCulture ), segment, StringComparison.OrdinalIgnoreCase ) )
                    return nonGenericDictionary[key];
            }

            return null;
        }

        return ReportFunctionCompiler.GetValue( item, segment );
    }

    private static bool TryResolvePathSegment( object item, string segment, out object value )
    {
        value = null;

        if ( item is null || string.IsNullOrWhiteSpace( segment ) )
            return false;

        if ( item is IEnumerable enumerable and not string and not IDictionary )
        {
            var values = new List<object>();
            var resolved = false;

            foreach ( var childItem in enumerable )
            {
                if ( !TryResolvePathSegment( childItem, segment, out var childValue ) )
                    continue;

                resolved = true;

                if ( childValue is IEnumerable childEnumerable and not string and not IDictionary )
                    values.AddRange( childEnumerable.Cast<object>() );
                else
                    values.Add( childValue );
            }

            value = values;

            return resolved;
        }

        if ( item is IDictionary<string, object> dictionary )
        {
            var key = dictionary.Keys.FirstOrDefault( x => string.Equals( x, segment, StringComparison.OrdinalIgnoreCase ) );

            return key is not null && dictionary.TryGetValue( key, out value );
        }

        if ( item is IDictionary nonGenericDictionary )
        {
            foreach ( var key in nonGenericDictionary.Keys )
            {
                if ( string.Equals( Convert.ToString( key, CultureInfo.InvariantCulture ), segment, StringComparison.OrdinalIgnoreCase ) )
                {
                    value = nonGenericDictionary[key];
                    return true;
                }
            }

            return false;
        }

        return ReportFunctionCompiler.TryGetValue( item, segment, out value );
    }

    #endregion
}