#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.PivotGrid.Utilities;

internal sealed class PivotGridObjectEqualityComparer : IEqualityComparer<object>
{
    public static readonly PivotGridObjectEqualityComparer Instance = new();

    private PivotGridObjectEqualityComparer()
    {
    }

    public new bool Equals( object x, object y )
        => object.Equals( x, y );

    public int GetHashCode( object obj )
        => obj?.GetHashCode() ?? 0;
}