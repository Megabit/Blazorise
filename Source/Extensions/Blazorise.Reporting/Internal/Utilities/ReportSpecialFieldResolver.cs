#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportSpecialFieldResolver
{
    #region Members

    internal const string DataSourceName = "__SpecialFields";

    internal const string PageNumberFieldName = "PageNumber";

    internal const string TotalPagesFieldName = "TotalPages";

    internal const string PrintDateFieldName = "PrintDate";

    internal const string PrintTimeFieldName = "PrintTime";

    internal const string ReportTitleFieldName = "ReportTitle";

    private static readonly IReadOnlyList<ReportSpecialFieldDefinition> fields =
    [
        new( PageNumberFieldName, "Page Number", typeof( int ) ),
        new( TotalPagesFieldName, "Total Pages", typeof( int ) ),
        new( PrintDateFieldName, "Print Date", typeof( DateTime ) ),
        new( PrintTimeFieldName, "Print Time", typeof( DateTime ) ),
        new( ReportTitleFieldName, "Report Title", typeof( string ) ),
    ];

    #endregion

    #region Methods

    internal static IReadOnlyList<ReportSpecialFieldDefinition> GetFields()
    {
        return fields;
    }

    internal static bool IsSpecialDataSource( string dataSourceName )
    {
        return string.Equals( dataSourceName, DataSourceName, StringComparison.OrdinalIgnoreCase );
    }

    internal static bool IsSpecialField( string fieldName )
    {
        return !string.IsNullOrWhiteSpace( fieldName )
            && fields.Any( field => string.Equals( field.Name, fieldName.Trim(), StringComparison.OrdinalIgnoreCase ) );
    }

    internal static bool TryResolve( string dataSourceName, string fieldName, ReportDefinition definition, out object value )
    {
        value = null;

        if ( string.IsNullOrWhiteSpace( fieldName ) )
            return false;

        var normalizedFieldName = fieldName.Trim();

        if ( !IsSpecialDataSource( dataSourceName ) && !IsSpecialField( normalizedFieldName ) )
            return false;

        if ( string.Equals( normalizedFieldName, PageNumberFieldName, StringComparison.OrdinalIgnoreCase ) )
        {
            value = 1;
            return true;
        }

        if ( string.Equals( normalizedFieldName, TotalPagesFieldName, StringComparison.OrdinalIgnoreCase ) )
        {
            value = 1;
            return true;
        }

        if ( string.Equals( normalizedFieldName, PrintDateFieldName, StringComparison.OrdinalIgnoreCase ) )
        {
            value = DateTime.Today;
            return true;
        }

        if ( string.Equals( normalizedFieldName, PrintTimeFieldName, StringComparison.OrdinalIgnoreCase ) )
        {
            value = DateTime.Now;
            return true;
        }

        if ( string.Equals( normalizedFieldName, ReportTitleFieldName, StringComparison.OrdinalIgnoreCase ) )
        {
            value = definition?.Name;
            return true;
        }

        return false;
    }

    #endregion
}

internal sealed record ReportSpecialFieldDefinition( string Name, string DisplayName, Type DataType );