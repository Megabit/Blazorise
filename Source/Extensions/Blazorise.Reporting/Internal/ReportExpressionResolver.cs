#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportExpressionResolver
{
    #region Methods

    internal static object ResolveValue( ReportDefinition definition, object data, object item, string expression, string dataSource = null )
    {
        if ( string.IsNullOrWhiteSpace( expression ) )
            return null;

        var normalizedExpression = expression.Trim();

        if ( ReportSpecialFieldResolver.TryResolve( dataSource, normalizedExpression, definition, out var specialValue ) )
            return specialValue;

        if ( TryResolveCurrentItemValue( item, normalizedExpression, dataSource, out var currentItemValue ) )
            return currentItemValue;

        var contextItem = ResolveContextItem( definition, data, item, dataSource );

        return ReportDataResolver.ResolveDataSourceValue( definition, data, normalizedExpression, contextItem );
    }

    internal static object ResolveFieldValue( ReportDefinition definition, object data, object item, ReportElementDefinition element )
    {
        if ( element is null || string.IsNullOrWhiteSpace( element.Field ) )
            return null;

        if ( element.Aggregate is not null )
            return ReportAggregateResolver.ResolveAggregateValue( definition, data, item, element );

        if ( ReportSpecialFieldResolver.TryResolve( element.DataSource, element.Field, definition, out var specialValue ) )
            return specialValue;

        var contextItem = string.IsNullOrWhiteSpace( element.DataSource )
            ? item
            : ReportDataResolver.ResolveItems( definition, data, element.DataSource, item ).FirstOrDefault() ?? item;

        return ResolveValue( definition, data, contextItem, element.Field );
    }

    private static object ResolveContextItem( ReportDefinition definition, object data, object item, string dataSource )
    {
        return string.IsNullOrWhiteSpace( dataSource )
            ? item
            : ReportDataResolver.ResolveDataSourceValue( definition, data, dataSource, item ) ?? item;
    }

    private static bool TryResolveCurrentItemValue( object item, string expression, string dataSource, out object value )
    {
        value = null;

        if ( item is null || string.IsNullOrWhiteSpace( expression ) )
            return false;

        foreach ( var candidate in GetCurrentItemExpressionCandidates( expression, dataSource ) )
        {
            if ( ReportDataResolver.TryResolvePathValue( item, candidate, out value ) )
                return true;
        }

        return false;
    }

    private static IEnumerable<string> GetCurrentItemExpressionCandidates( string expression, string dataSource )
    {
        yield return expression;

        if ( string.IsNullOrWhiteSpace( dataSource ) )
            yield break;

        var normalizedDataSource = dataSource.Trim();
        var dataSourcePrefix = $"{normalizedDataSource}.";

        if ( expression.StartsWith( dataSourcePrefix, StringComparison.OrdinalIgnoreCase ) )
            yield return expression[dataSourcePrefix.Length..];

        var dataSourceLeaf = normalizedDataSource.Split( '.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries ).LastOrDefault();

        if ( string.IsNullOrWhiteSpace( dataSourceLeaf ) )
            yield break;

        var dataSourceLeafPrefix = $"{dataSourceLeaf}.";

        if ( !string.Equals( dataSourceLeafPrefix, dataSourcePrefix, StringComparison.OrdinalIgnoreCase )
            && expression.StartsWith( dataSourceLeafPrefix, StringComparison.OrdinalIgnoreCase ) )
        {
            yield return expression[dataSourceLeafPrefix.Length..];
        }
    }

    #endregion
}