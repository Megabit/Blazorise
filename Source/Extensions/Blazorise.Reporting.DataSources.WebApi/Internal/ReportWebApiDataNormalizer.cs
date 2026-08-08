#region Using directives
using System.Collections;
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting.DataSources.WebApi;

internal static class ReportWebApiDataNormalizer
{
    #region Methods

    internal static object NormalizeRoot( object value )
    {
        if ( value is IEnumerable enumerable and not string and not IDictionary )
        {
            List<object> items = [];

            foreach ( object item in enumerable )
            {
                items.Add( NormalizeRecord( item ) );
            }

            return items;
        }

        return NormalizeRecord( value );
    }

    private static object NormalizeRecord( object value )
    {
        if ( value is IDictionary )
            return value;

        return new Dictionary<string, object>( System.StringComparer.OrdinalIgnoreCase )
        {
            ["Value"] = value,
        };
    }

    #endregion
}