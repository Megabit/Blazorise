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

        var fieldExpression = FormatFieldExpression( element.DataSource, element.Field );

        return FormatAggregateExpression( element, fieldExpression );
    }

    internal static string FormatFieldExpression( ReportDefinition definition, ReportElementDefinition element )
    {
        if ( element is null || string.IsNullOrWhiteSpace( element.Field ) )
            return string.Empty;

        var fieldExpression = FormatFieldExpression( ResolveDisplayDataSourceName( definition, element.DataSource ), element.Field );

        return FormatAggregateExpression( element, fieldExpression );
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

    private static string FormatAggregateExpression( ReportElementDefinition element, string fieldExpression )
    {
        return element.Aggregate is null
            ? fieldExpression
            : $"{ReportAggregateResolver.GetFunctionDisplayName( element.Aggregate.Function )}({fieldExpression})";
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
        string normalizedDataSourceName = dataSourceName.Trim();

        if ( string.IsNullOrWhiteSpace( rootDataSourceName ) )
            return normalizedDataSourceName;

        string rootDataSourcePrefix = $"{rootDataSourceName.Trim()}.";

        if ( string.Equals( normalizedDataSourceName, rootDataSourceName, StringComparison.OrdinalIgnoreCase ) )
            return null;

        return normalizedDataSourceName.StartsWith( rootDataSourcePrefix, StringComparison.OrdinalIgnoreCase )
            ? normalizedDataSourceName[rootDataSourcePrefix.Length..]
            : normalizedDataSourceName;
    }

    #endregion
}