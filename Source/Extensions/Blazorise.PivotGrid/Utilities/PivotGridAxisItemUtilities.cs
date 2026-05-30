#region Using directives
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.PivotGrid.Utilities;

internal static class PivotGridAxisItemUtilities
{
    internal static bool CanToggleExpansion<TItem>( PivotGridAxisItem<TItem> axisItem, IReadOnlyList<PivotGridFieldInfo<TItem>> axisFields )
        => axisItem is not null
            && axisFields is not null
            && axisItem.IsTotal
            && !axisItem.IsGrandTotal
            && axisItem.Values.Count > 0
            && axisItem.Values.Count < axisFields.Count;

    internal static bool IsInGroupPage<TItem>( PivotGridAxisItem<TItem> row, HashSet<string> pageRootGroupKeys )
    {
        if ( row.IsGrandTotal )
            return true;

        if ( row.Values.Count == 0 )
            return false;

        return pageRootGroupKeys.Contains( PivotGridKeyGenerator.CreateGroupKey( row.Values.Take( 1 ) ) );
    }
}