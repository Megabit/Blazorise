#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.PivotGrid.Utilities;

internal sealed class PivotGridAxisItemEqualityComparer<TItem> : IEqualityComparer<PivotGridAxisItem<TItem>>
{
    public static readonly PivotGridAxisItemEqualityComparer<TItem> Instance = new();

    private PivotGridAxisItemEqualityComparer()
    {
    }

    public bool Equals( PivotGridAxisItem<TItem> x, PivotGridAxisItem<TItem> y )
    {
        if ( ReferenceEquals( x, y ) )
            return true;

        if ( x is null || y is null )
            return false;

        if ( x.Level != y.Level || x.IsTotal != y.IsTotal || x.IsGrandTotal != y.IsGrandTotal || x.Values.Count != y.Values.Count )
            return false;

        for ( int i = 0; i < x.Values.Count; i++ )
        {
            if ( !object.Equals( x.Values[i], y.Values[i] ) )
                return false;
        }

        return true;
    }

    public int GetHashCode( PivotGridAxisItem<TItem> obj )
    {
        if ( obj is null )
            return 0;

        int hashCode = 17;

        hashCode = ( hashCode * 31 ) + obj.Level.GetHashCode();
        hashCode = ( hashCode * 31 ) + obj.IsTotal.GetHashCode();
        hashCode = ( hashCode * 31 ) + obj.IsGrandTotal.GetHashCode();

        foreach ( object value in obj.Values )
        {
            hashCode = ( hashCode * 31 ) + ( value?.GetHashCode() ?? 0 );
        }

        return hashCode;
    }
}