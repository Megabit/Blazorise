#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
#endregion

namespace Blazorise.PivotGrid.Utilities;

internal static class PivotGridKeyGenerator
{
    internal static string CreateDataRequestKey( PivotGridDataRequest request )
    {
        StringBuilder builder = new();

        AppendValueKey( builder, request.ReadDataMode );
        AppendValueKey( builder, request.Page );
        AppendValueKey( builder, request.PageSize );
        AppendValueKey( builder, request.VirtualizeOffset );
        AppendValueKey( builder, request.VirtualizeCount );
        AppendValueKey( builder, request.PageByGroups );
        AppendValueKey( builder, request.ShowPager );
        AppendValueKey( builder, request.ShowRowSubtotals );
        AppendValueKey( builder, request.ShowColumnSubtotals );
        AppendValueKey( builder, request.ShowRowTotals );
        AppendValueKey( builder, request.ShowColumnTotals );
        AppendValueKey( builder, request.RowTotalPosition );
        AppendValueKey( builder, request.ColumnTotalPosition );
        AppendValueKey( builder, request.ExpandableRows );
        AppendValueKey( builder, request.ExpandableColumns );
        AppendValueKey( builder, request.InitiallyExpanded );
        AppendFieldStatesKey( builder, request.Rows );
        AppendFieldStatesKey( builder, request.Columns );
        AppendFieldStatesKey( builder, request.Aggregates );
        AppendFieldStatesKey( builder, request.Filters );

        return builder.ToString();
    }

    internal static string CreateFilterValueKey( object value )
        => CreateValueKey( value );

    internal static string CreateGroupKey( IEnumerable<object> values )
    {
        StringBuilder builder = new();

        foreach ( object value in values )
        {
            AppendValueKey( builder, value );
        }

        return builder.ToString();
    }

    private static void AppendFieldStatesKey( StringBuilder builder, IReadOnlyList<PivotGridFieldState> states )
    {
        AppendValueKey( builder, states?.Count ?? 0 );

        if ( states is null )
            return;

        foreach ( PivotGridFieldState state in states )
        {
            AppendValueKey( builder, state.Field );
            AppendValueKey( builder, state.Caption );
            AppendValueKey( builder, state.FieldType?.AssemblyQualifiedName ?? state.FieldType?.FullName );
            AppendValueKey( builder, state.Area );
            AppendValueKey( builder, state.Aggregate );
            AppendValueKey( builder, state.FilterValueKey );
        }
    }

    private static string CreateValueKey( object value )
    {
        StringBuilder builder = new();

        AppendValueKey( builder, value );

        return builder.ToString();
    }

    private static void AppendValueKey( StringBuilder builder, object value )
    {
        // Length-prefix both the value type and formatted value so group/filter/request keys cannot collide
        // when values contain separators or different value types format to the same text.
        if ( value is null )
        {
            builder.Append( "n;" );
            return;
        }

        Type valueType = value.GetType();
        string typeName = valueType.AssemblyQualifiedName ?? valueType.FullName ?? valueType.Name;
        string text = value is IFormattable formattable
            ? formattable.ToString( null, CultureInfo.InvariantCulture )
            : value.ToString() ?? string.Empty;

        builder
            .Append( "v;" )
            .Append( typeName.Length.ToString( CultureInfo.InvariantCulture ) )
            .Append( ';' )
            .Append( typeName )
            .Append( text.Length.ToString( CultureInfo.InvariantCulture ) )
            .Append( ';' )
            .Append( text );
    }
}