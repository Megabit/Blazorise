#region Using directives
using System.Collections;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportTextTemplateResolver
{
    #region Methods

    internal static string ResolveText( ReportDefinition definition, object data, object item, ReportElementDefinition element, IReadOnlyDictionary<string, object> runningTotals = null )
    {
        if ( string.IsNullOrEmpty( element?.Text ) )
            return string.Empty;

        return ResolveText( definition, data, item, element.Text, element.DataSource, runningTotals );
    }

    internal static string ResolveText( ReportDefinition definition, object data, object item, string text, string dataSource = null, IReadOnlyDictionary<string, object> runningTotals = null )
    {
        if ( string.IsNullOrEmpty( text ) )
            return string.Empty;

        var builder = new StringBuilder();
        var index = 0;

        while ( index < text.Length )
        {
            var openIndex = text.IndexOf( '{', index );

            if ( openIndex < 0 )
            {
                builder.Append( text, index, text.Length - index );
                break;
            }

            var closeIndex = text.IndexOf( '}', openIndex + 1 );

            if ( closeIndex < 0 )
            {
                builder.Append( text, index, text.Length - index );
                break;
            }

            builder.Append( text, index, openIndex - index );

            var expression = text[( openIndex + 1 )..closeIndex].Trim();

            if ( expression.Length == 0 )
                builder.Append( text, openIndex, closeIndex - openIndex + 1 );
            else
                builder.Append( ResolveExpression( definition, data, item, expression, dataSource, runningTotals ) );

            index = closeIndex + 1;
        }

        return builder.ToString();
    }

    private static string ResolveExpression( ReportDefinition definition, object data, object item, string expression, string dataSource, IReadOnlyDictionary<string, object> runningTotals )
    {
        var value = ReportExpressionResolver.ResolveValue( definition, data, item, expression, dataSource, runningTotals );

        return FormatTemplateValue( value );
    }

    private static string FormatTemplateValue( object value )
    {
        if ( value is IEnumerable enumerable and not string and not IDictionary )
        {
            var values = new List<string>();

            foreach ( var item in enumerable )
            {
                values.Add( ReportDataResolver.FormatValue( item, null ) );
            }

            return string.Join( ", ", values );
        }

        return ReportDataResolver.FormatValue( value, null );
    }

    #endregion
}