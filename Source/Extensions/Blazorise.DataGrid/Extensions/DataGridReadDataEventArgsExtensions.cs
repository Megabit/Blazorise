using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazorise.Extensions;

namespace Blazorise.DataGrid.Extensions;

/// <summary>
/// Extension methods for <see cref="DataGridReadDataEventArgs{T}"/>.
/// </summary>
public static class DataGridReadDataEventArgsExtensions
{
    /// <summary>
    /// Builds an OData query string from the provided <see cref="DataGridReadDataEventArgs{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="args"></param>
    /// <param name="url"></param>
    /// <returns></returns>
    public static string ToODataString<T>( this DataGridReadDataEventArgs<T> args, string url )
    {
        var selects = new List<string>();
        var filters = new List<string>();
        var orderBys = new List<string>();

        foreach ( var column in args.Columns )
        {
            if ( string.IsNullOrEmpty( column.Field ) )
            {
                continue;
            }

            var field = column.Field.Replace( '.', '/' );

            selects.Add( field );

            if ( column.SearchValue is not null )
            {
                var searchOperator = column.FilterMethod switch
                {
                    DataGridColumnFilterMethod.GreaterThan => "gt",
                    DataGridColumnFilterMethod.GreaterThanOrEqual => "ge",
                    DataGridColumnFilterMethod.LessThan => "lt",
                    DataGridColumnFilterMethod.LessThanOrEqual => "le",
                    DataGridColumnFilterMethod.Equals => "eq",
                    DataGridColumnFilterMethod.NotEquals => "ne",
                    _ => null,
                };
                if ( searchOperator is not null )
                {
                    filters.Add( $"{field} {searchOperator} '{column.SearchValue}'" );
                }
                else
                {
                    var searchFunction = column.FilterMethod switch
                    {
                        DataGridColumnFilterMethod.Contains => "contains",
                        DataGridColumnFilterMethod.StartsWith => "startswith",
                        DataGridColumnFilterMethod.EndsWith => "endswith",
                        _ => null,
                    };

                    if ( searchFunction is not null )
                    {
                        filters.Add( $"{searchFunction}({field},'{column.SearchValue}')" );
                    }
                }
            }
        }

        foreach ( var column in args.Columns.Where( col => !string.IsNullOrEmpty( col.SortField ) && col.SortIndex >= 0 ).OrderBy( col => col.SortIndex ) )
        {
            var direction = column.SortDirection == SortDirection.Ascending ? "asc" : "desc";
            orderBys.Add( $"{column.SortField.Replace( '.', '/' )} {direction}" );
        }

        var selectQuery = selects.Any() ? $"select={string.Join( ",", selects )}" : string.Empty;
        var filterQuery = filters.Any() ? $"$filter={string.Join( " and ", filters )}" : string.Empty;
        var orderByQuery = orderBys.Any() ? $"$orderby={string.Join( ",", orderBys )}" : string.Empty;
        var skipQuery = args.Page > 0 ? $"$skip={( args.Page - 1 ) * args.PageSize}" : string.Empty;
        var topQuery = args.PageSize > 0 ? $"$top={args.PageSize}" : string.Empty;
        var countQuery = "$count=true";

        var querySegments = new[] { selectQuery, filterQuery, orderByQuery, skipQuery, topQuery, countQuery }
            .Where( segment => !string.IsNullOrEmpty( segment ) );

        return $"{url}?{string.Join( "&", querySegments )}";
    }

}
