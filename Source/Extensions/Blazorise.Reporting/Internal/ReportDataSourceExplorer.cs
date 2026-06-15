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
        if ( definition?.DataSources is null )
            yield break;

        foreach ( var dataSource in definition.DataSources )
        {
            List<ReportDesignerFieldNode> fields = ResolveDataSourceFields( dataSource ).ToList();

            if ( fields.Count == 0 )
                continue;

            yield return new()
            {
                Name = string.IsNullOrWhiteSpace( dataSource.Name ) ? defaultDataSourceName : dataSource.Name,
                Fields = fields,
            };
        }
    }

    internal static IEnumerable<ReportDesignerDataSourceOption> ResolveBindableDataSources( ReportDefinition definition )
    {
        if ( definition?.DataSources is null )
            yield break;

        HashSet<string> usedValues = new( StringComparer.OrdinalIgnoreCase );

        foreach ( ReportDataSourceDefinition dataSource in definition.DataSources )
        {
            if ( string.IsNullOrWhiteSpace( dataSource?.Name ) )
                continue;

            if ( IsBindableCollectionDataSource( dataSource ) && usedValues.Add( dataSource.Name ) )
            {
                yield return new()
                {
                    Value = dataSource.Name,
                    DisplayName = dataSource.Name,
                };
            }

            foreach ( ReportDesignerFieldNode field in ResolveDataSourceFields( dataSource ) )
            {
                foreach ( ReportDesignerDataSourceOption option in ResolveBindableFieldDataSources( dataSource.Name, field, usedValues ) )
                {
                    yield return option;
                }
            }
        }
    }

    internal static IEnumerable<ReportDesignerFieldNode> ResolveDataSourceFields( ReportDataSourceDefinition dataSource )
    {
        if ( dataSource?.Schema is not null )
        {
            foreach ( ReportDataSourceSchemaField field in dataSource.Schema.Fields ?? [] )
            {
                if ( field is null )
                    continue;

                yield return CreateDesignerFieldNode( field, null );
            }

            yield break;
        }

        foreach ( ReportDesignerFieldNode field in ResolveDataSourceFields( dataSource?.Data ) )
        {
            yield return field;
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

    internal static bool TryResolveFieldType( ReportDefinition definition, object defaultData, string dataSourceName, string fieldName, out Type dataType )
    {
        dataType = null;

        if ( string.IsNullOrWhiteSpace( fieldName ) )
            return false;

        if ( ReportSpecialFieldResolver.IsSpecialDataSource( dataSourceName ) || ReportSpecialFieldResolver.IsSpecialField( fieldName ) )
        {
            ReportSpecialFieldDefinition specialField = ReportSpecialFieldResolver.GetFields().FirstOrDefault( field =>
                string.Equals( field.Name, fieldName.Trim(), StringComparison.OrdinalIgnoreCase ) );

            dataType = specialField?.DataType;

            return dataType is not null;
        }

        if ( ReportFormulaFieldResolver.IsFormulaDataSource( dataSourceName ) || ReportFormulaFieldResolver.IsFormulaField( definition, fieldName ) )
        {
            dataType = typeof( object );
            return true;
        }

        List<string> normalizedFieldNames = NormalizeFieldPathCandidates( definition, dataSourceName, fieldName ).ToList();
        object dataSourceValue = ReportDataResolver.ResolveDataSourceValue( definition, defaultData, dataSourceName );
        List<ReportDesignerFieldNode> fields = ResolveDataSourceFields( dataSourceValue ).ToList();

        if ( fields.Count == 0 )
            fields = ResolveDataSourceSchemaContextFields( definition, dataSourceName ).ToList();

        ReportDesignerFieldNode fieldNode = fields.FirstOrDefault( field =>
            normalizedFieldNames.Any( normalizedFieldName =>
                string.Equals( field.Path, normalizedFieldName, StringComparison.OrdinalIgnoreCase )
                || string.Equals( field.Name, normalizedFieldName, StringComparison.OrdinalIgnoreCase ) ) );

        dataType = fieldNode?.DataType;

        return dataType is not null;
    }

    private static IEnumerable<ReportDesignerFieldNode> ResolveDataSourceSchemaContextFields( ReportDefinition definition, string dataSourceName )
    {
        if ( definition?.DataSources is null )
            yield break;

        if ( string.IsNullOrWhiteSpace( dataSourceName ) )
        {
            ReportDataSourceDefinition defaultDataSource = definition.DataSources.FirstOrDefault( dataSource => dataSource?.Schema is not null );

            foreach ( ReportDesignerFieldNode field in ResolveDataSourceFields( defaultDataSource ) )
            {
                yield return field;
            }

            yield break;
        }

        string normalizedDataSourceName = dataSourceName.Trim();

        foreach ( ReportDataSourceDefinition dataSource in definition.DataSources.Where( dataSource => dataSource?.Schema is not null ) )
        {
            if ( string.Equals( dataSource.Name, normalizedDataSourceName, StringComparison.OrdinalIgnoreCase ) )
            {
                foreach ( ReportDesignerFieldNode field in ResolveDataSourceFields( dataSource ) )
                {
                    yield return field;
                }

                yield break;
            }

            string prefix = $"{dataSource.Name}.";
            string schemaPath = normalizedDataSourceName.StartsWith( prefix, StringComparison.OrdinalIgnoreCase )
                ? normalizedDataSourceName[prefix.Length..]
                : normalizedDataSourceName;

            ReportDesignerFieldNode schemaNode = FindFieldNodeByPath( ResolveDataSourceFields( dataSource ), schemaPath );

            if ( schemaNode is not null )
            {
                foreach ( ReportDesignerFieldNode child in schemaNode.Children ?? [] )
                {
                    yield return child;
                }

                yield break;
            }
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
            var value = ReportFunctionCompiler.GetValue( item, property.Name );

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
            IsCollection = IsEnumerableData( dataType ) || IsEnumerableData( value?.GetType() ),
        };

        if ( sampleValue is not null && !IsSimpleFieldType( sampleValue.GetType() ) )
            node.Children = ResolveDataSourceFields( sampleValue, path, depth + 1, visitedTypes ).ToList();

        return node;
    }

    private static IEnumerable<ReportDesignerDataSourceOption> ResolveBindableFieldDataSources( string dataSourceName, ReportDesignerFieldNode field, HashSet<string> usedValues )
    {
        if ( field is null )
            yield break;

        if ( field.IsCollection )
        {
            string value = $"{dataSourceName}.{field.Path}";

            if ( usedValues.Add( value ) )
            {
                yield return new()
                {
                    Value = value,
                    DisplayName = field.Path,
                };
            }
        }

        foreach ( ReportDesignerFieldNode child in field.Children ?? [] )
        {
            if ( child is null )
                continue;

            foreach ( ReportDesignerDataSourceOption option in ResolveBindableFieldDataSources( dataSourceName, child, usedValues ) )
            {
                yield return option;
            }
        }
    }

    private static Type GetDesignerFieldDataType( Type declaredType, object sampleValue )
    {
        var enumerableItemType = GetEnumerableItemType( declaredType );

        return enumerableItemType ?? sampleValue?.GetType() ?? declaredType;
    }

    private static ReportDesignerFieldNode FindFieldNodeByPath( IEnumerable<ReportDesignerFieldNode> fields, string path )
    {
        if ( string.IsNullOrWhiteSpace( path ) )
            return null;

        foreach ( ReportDesignerFieldNode field in fields ?? [] )
        {
            if ( string.Equals( field.Path, path, StringComparison.OrdinalIgnoreCase ) )
                return field;

            ReportDesignerFieldNode child = FindFieldNodeByPath( field.Children, path );

            if ( child is not null )
                return child;
        }

        return null;
    }

    private static IEnumerable<string> NormalizeFieldPathCandidates( ReportDefinition definition, string dataSourceName, string fieldName )
    {
        string normalizedFieldName = fieldName.Trim();

        yield return normalizedFieldName;

        foreach ( string normalizedDataSourceFieldName in NormalizeDataSourceFieldPathCandidates( dataSourceName, normalizedFieldName ) )
        {
            yield return normalizedDataSourceFieldName;
        }

        string rootDataSourceName = definition?.DataSources.FirstOrDefault()?.Name;

        if ( string.IsNullOrWhiteSpace( rootDataSourceName ) )
            yield break;

        string rootDataSourcePrefix = $"{rootDataSourceName.Trim()}.";

        if ( !normalizedFieldName.StartsWith( rootDataSourcePrefix, StringComparison.OrdinalIgnoreCase ) )
            yield break;

        string rootRelativeFieldName = normalizedFieldName[rootDataSourcePrefix.Length..];

        yield return rootRelativeFieldName;

        foreach ( string normalizedDataSourceFieldName in NormalizeDataSourceFieldPathCandidates( dataSourceName, rootRelativeFieldName ) )
        {
            yield return normalizedDataSourceFieldName;
        }
    }

    private static IEnumerable<string> NormalizeDataSourceFieldPathCandidates( string dataSourceName, string fieldName )
    {
        if ( string.IsNullOrWhiteSpace( dataSourceName ) || string.IsNullOrWhiteSpace( fieldName ) )
            yield break;

        string normalizedDataSourceName = dataSourceName.Trim();
        string dataSourcePrefix = $"{normalizedDataSourceName}.";

        if ( fieldName.StartsWith( dataSourcePrefix, StringComparison.OrdinalIgnoreCase ) )
            yield return fieldName[dataSourcePrefix.Length..];

        string dataSourceLeaf = normalizedDataSourceName.Split( '.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries ).LastOrDefault();

        if ( string.IsNullOrWhiteSpace( dataSourceLeaf ) )
            yield break;

        string dataSourceLeafPrefix = $"{dataSourceLeaf}.";

        if ( fieldName.StartsWith( dataSourceLeafPrefix, StringComparison.OrdinalIgnoreCase ) )
            yield return fieldName[dataSourceLeafPrefix.Length..];
    }

    internal static bool IsEnumerableData( Type type )
    {
        return GetEnumerableItemType( type ) is not null;
    }

    private static ReportDesignerFieldNode CreateDesignerFieldNode( ReportDataSourceSchemaField field, string parentPath )
    {
        string path = string.IsNullOrWhiteSpace( parentPath ) ? field.Name : $"{parentPath}.{field.Name}";

        return new()
        {
            Name = field.Name,
            Path = path,
            DataType = field.DataType,
            IsCollection = field.IsCollection,
            Children = field.Fields?
                .Where( child => child is not null )
                .Select( child => CreateDesignerFieldNode( child, path ) )
                .ToList() ?? [],
        };
    }

    private static bool IsBindableCollectionDataSource( ReportDataSourceDefinition dataSource )
    {
        return dataSource?.Schema?.IsCollection == true
            || IsEnumerableData( dataSource?.Data?.GetType() );
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