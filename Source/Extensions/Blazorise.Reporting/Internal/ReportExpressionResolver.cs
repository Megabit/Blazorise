#region Using directives
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

        var contextItem = ResolveContextItem( definition, data, item, dataSource );

        return ReportDataResolver.ResolveDataSourceValue( definition, data, expression.Trim(), contextItem );
    }

    internal static object ResolveFieldValue( ReportDefinition definition, object data, object item, ReportElementDefinition element )
    {
        if ( element is null || string.IsNullOrWhiteSpace( element.Field ) )
            return null;

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

    #endregion
}