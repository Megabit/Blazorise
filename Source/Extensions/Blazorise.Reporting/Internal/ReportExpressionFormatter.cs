#region Using directives
using System;
using System.Linq;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportExpressionFormatter
{
    #region Methods

    internal static string FormatFieldExpression( ReportElementDefinition element )
    {
        if ( element is null || string.IsNullOrWhiteSpace( element.Field ) )
            return string.Empty;

        return FormatFieldExpression( element.DataSource, element.Field );
    }

    internal static string FormatFieldExpression( ReportDefinition definition, ReportElementDefinition element )
    {
        if ( element is null || string.IsNullOrWhiteSpace( element.Field ) )
            return string.Empty;

        return FormatFieldExpression( ResolveDisplayDataSourceName( definition, element.DataSource ), element.Field );
    }

    internal static string FormatFieldExpression( string dataSourceName, string fieldName )
    {
        if ( string.IsNullOrWhiteSpace( fieldName ) )
            return string.Empty;

        return $"{{{FormatFieldPath( dataSourceName, fieldName )}}}";
    }

    internal static string FormatFieldPath( string dataSourceName, string fieldName )
    {
        if ( string.IsNullOrWhiteSpace( fieldName ) )
            return string.Empty;

        string normalizedFieldName = fieldName.Trim();

        if ( string.IsNullOrWhiteSpace( dataSourceName ) )
            return normalizedFieldName;

        string normalizedDataSourceName = dataSourceName.Trim();
        string dataSourcePrefix = $"{normalizedDataSourceName}.";

        return normalizedFieldName.StartsWith( dataSourcePrefix, StringComparison.OrdinalIgnoreCase )
            ? normalizedFieldName
            : $"{normalizedDataSourceName}.{normalizedFieldName}";
    }

    internal static void AppendFieldExpressionToText( ReportElementDefinition element, string dataSourceName, string fieldName )
    {
        if ( element is null || element.Type != ReportElementType.Text )
            return;

        string expression = FormatFieldExpression( dataSourceName, fieldName );

        if ( string.IsNullOrWhiteSpace( expression ) )
            return;

        element.Text = string.IsNullOrEmpty( element.Text )
            ? expression
            : HasTrailingExpressionSeparator( element.Text )
                ? $"{element.Text}{expression}"
                : $"{element.Text} {expression}";
    }

    private static bool HasTrailingExpressionSeparator( string text )
    {
        return text.EndsWith( " ", StringComparison.Ordinal )
            || text.EndsWith( Environment.NewLine, StringComparison.Ordinal );
    }

    private static string ResolveDisplayDataSourceName( ReportDefinition definition, string dataSourceName )
    {
        if ( string.IsNullOrWhiteSpace( dataSourceName ) )
            return null;

        string rootDataSourceName = definition?.DataSources.FirstOrDefault()?.Name;

        return !string.IsNullOrWhiteSpace( rootDataSourceName ) && string.Equals( dataSourceName, rootDataSourceName, StringComparison.OrdinalIgnoreCase )
            ? null
            : dataSourceName;
    }

    #endregion
}