#region Using directives
using System.Threading.Tasks;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Maps;

internal class MapAdapter
{
    private readonly Map map;

    public MapAdapter( Map map )
    {
        this.map = map;
    }

    [JSInvokable( "Click" )]
    public ValueTask Click( MapMouseEventArgs eventArgs )
        => map.NotifyClicked( eventArgs );

    [JSInvokable( "DoubleClick" )]
    public ValueTask DoubleClick( MapMouseEventArgs eventArgs )
        => map.NotifyDoubleClicked( eventArgs );

    [JSInvokable( "ContextMenu" )]
    public ValueTask ContextMenu( MapMouseEventArgs eventArgs )
        => map.NotifyContextMenu( eventArgs );

    [JSInvokable( "ViewChanged" )]
    public ValueTask ViewChanged( MapViewChangedEventArgs eventArgs )
        => map.NotifyViewChanged( eventArgs );

    [JSInvokable( "MarkerClick" )]
    public ValueTask MarkerClick( string layerId, string itemId, MapMouseEventArgs eventArgs )
        => map.NotifyMarkerClicked( layerId, itemId, eventArgs );

    [JSInvokable( "MarkerDragged" )]
    public ValueTask MarkerDragged( string layerId, string itemId, MapCoordinate coordinate )
        => map.NotifyMarkerDragged( layerId, itemId, coordinate );
}