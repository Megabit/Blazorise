namespace Blazorise.Maps;

internal static class MapViewUtils
{
    public static bool IsSameView( MapView first, MapView second )
    {
        if ( first is null && second is null )
            return true;

        if ( first is null || second is null )
            return false;

        return first.Center.Equals( second.Center )
            && first.Zoom.Equals( second.Zoom );
    }

    public static MapView CloneView( MapView view )
    {
        if ( view is null )
            return null;

        return new()
        {
            Center = view.Center,
            Zoom = view.Zoom,
            Bounds = view.Bounds,
        };
    }
}