#region Using directives
using System.Linq;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportDataSourceSchemaBuilder
{
    #region Methods

    internal static ReportDataSourceSchema FromData( object data )
    {
        return new()
        {
            IsCollection = ReportDataSourceExplorer.IsEnumerableData( data?.GetType() ),
            Fields = ReportDataSourceExplorer.ResolveDataSourceFields( data )
                .Select( CreateSchemaField )
                .ToList(),
        };
    }

    private static ReportDataSourceSchemaField CreateSchemaField( ReportDesignerFieldNode field )
    {
        return new()
        {
            Name = field.Name,
            DisplayName = field.Name,
            DataType = field.DataType,
            IsCollection = field.IsCollection,
            Fields = field.Children?
                .Where( child => child is not null )
                .Select( CreateSchemaField )
                .ToList() ?? [],
        };
    }

    #endregion
}