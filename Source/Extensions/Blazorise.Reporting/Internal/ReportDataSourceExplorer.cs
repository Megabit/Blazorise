#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportDataSourceExplorer
{
    #region Methods

    internal static IEnumerable<ReportDesignerDataSourceNode> ResolveDataSourceDictionary( ReportDefinition definition, string defaultDataSourceName )
    {
        foreach ( var dataSource in definition.DataSources )
        {
            var fields = ResolveDataSourceFields( dataSource.Data ).ToList();

            if ( fields.Count == 0 )
                continue;

            yield return new()
            {
                Name = string.IsNullOrWhiteSpace( dataSource.Name ) ? defaultDataSourceName : dataSource.Name,
                Fields = fields,
            };
        }
    }

    internal static IEnumerable<ReportDesignerFieldNode> ResolveDataSourceFields( object data )
    {
        var item = ResolveSampleItem( data );

        if ( item is null )
            yield break;

        foreach ( var field in ResolveDataSourceFields( item, null, 0, [] ) )
        {
            yield return field;
        }
    }

    private static IEnumerable<ReportDesignerFieldNode> ResolveDataSourceFields( object item, string parentPath, int depth, HashSet<Type> visitedTypes )
    {
        if ( item is null || depth > 4 )
            yield break;

        if ( item is IDictionary<string, object> dictionary )
        {
            foreach ( var key in dictionary.Keys.OrderBy( x => x ) )
            {
                dictionary.TryGetValue( key, out var value );

                yield return CreateDesignerFieldNode( key, parentPath, value?.GetType(), value, depth, visitedTypes );
            }

            yield break;
        }

        if ( item is IDictionary nonGenericDictionary )
        {
            foreach ( var key in nonGenericDictionary.Keys
                .OfType<object>()
                .Select( key => new { Key = key, Name = Convert.ToString( key, CultureInfo.InvariantCulture ) } )
                .Where( x => !string.IsNullOrWhiteSpace( x.Name ) )
                .OrderBy( x => x.Name ) )
            {
                var value = nonGenericDictionary[key.Key];

                yield return CreateDesignerFieldNode( key.Name, parentPath, value?.GetType(), value, depth, visitedTypes );
            }

            yield break;
        }

        var itemType = item.GetType();

        if ( IsSimpleFieldType( itemType ) || !visitedTypes.Add( itemType ) )
            yield break;

        foreach ( var property in itemType.GetProperties( BindingFlags.Instance | BindingFlags.Public )
            .Where( x => x.CanRead && x.GetIndexParameters().Length == 0 )
            .OrderBy( x => x.Name ) )
        {
            var value = property.GetValue( item );

            yield return CreateDesignerFieldNode( property.Name, parentPath, property.PropertyType, value, depth, visitedTypes );
        }

        visitedTypes.Remove( itemType );
    }

    private static ReportDesignerFieldNode CreateDesignerFieldNode( string name, string parentPath, Type dataType, object value, int depth, HashSet<Type> visitedTypes )
    {
        var path = string.IsNullOrWhiteSpace( parentPath ) ? name : $"{parentPath}.{name}";
        var sampleValue = ResolveSampleItem( value );
        var node = new ReportDesignerFieldNode
        {
            Name = name,
            Path = path,
            DataType = GetDesignerFieldDataType( dataType, sampleValue ),
        };

        if ( sampleValue is not null && !IsSimpleFieldType( sampleValue.GetType() ) )
            node.Children = ResolveDataSourceFields( sampleValue, path, depth + 1, visitedTypes ).ToList();

        return node;
    }

    private static Type GetDesignerFieldDataType( Type declaredType, object sampleValue )
    {
        var enumerableItemType = GetEnumerableItemType( declaredType );

        return enumerableItemType ?? sampleValue?.GetType() ?? declaredType;
    }

    private static Type GetEnumerableItemType( Type type )
    {
        if ( type is null || type == typeof( string ) )
            return null;

        if ( type.IsArray )
            return type.GetElementType();

        if ( type.IsGenericType && type.GetGenericTypeDefinition() == typeof( IEnumerable<> ) )
            return type.GetGenericArguments()[0];

        return type.GetInterfaces()
            .FirstOrDefault( x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof( IEnumerable<> ) )
            ?.GetGenericArguments()[0];
    }

    private static bool IsSimpleFieldType( Type type )
    {
        type = Nullable.GetUnderlyingType( type ) ?? type;

        return type.IsPrimitive
            || type.IsEnum
            || type == typeof( string )
            || type == typeof( decimal )
            || type == typeof( DateTime )
            || type == typeof( DateTimeOffset )
            || type == typeof( TimeSpan )
            || type == typeof( Guid );
    }

    private static object ResolveSampleItem( object data )
    {
        if ( data is null || data is string )
            return data;

        if ( data is IEnumerable enumerable )
        {
            foreach ( var item in enumerable )
            {
                if ( item is not null )
                    return item;
            }

            return null;
        }

        return data;
    }

    #endregion
}