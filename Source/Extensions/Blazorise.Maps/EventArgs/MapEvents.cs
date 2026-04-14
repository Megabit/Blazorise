using System;

namespace Blazorise.Maps;

/// <summary>
/// Provides data for the map ready event.
/// </summary>
public class MapReadyEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of map ready event args.
    /// </summary>
    /// <param name="map">The map instance.</param>
    public MapReadyEventArgs( Map map )
    {
        Map = map;
    }

    /// <summary>
    /// References the initialized map instance.
    /// </summary>
    public Map Map { get; }
}

/// <summary>
/// Provides coordinates for map mouse interactions.
/// </summary>
public class MapMouseEventArgs : EventArgs
{
    /// <summary>
    /// Identifies the geographic coordinate where the interaction occurred.
    /// </summary>
    public MapCoordinate Coordinate { get; set; }

    /// <summary>
    /// Identifies the pointer position relative to the map container.
    /// </summary>
    public MapPoint ContainerPoint { get; set; }
}

/// <summary>
/// Provides data for map view changes.
/// </summary>
public class MapViewChangedEventArgs : EventArgs
{
    /// <summary>
    /// Contains the updated map view.
    /// </summary>
    public MapView View { get; set; }

    /// <summary>
    /// Contains the visible geographic bounds after the change.
    /// </summary>
    public MapBounds Bounds { get; set; }

    /// <summary>
    /// Identifies what caused the view change.
    /// </summary>
    public MapChangeReason Reason { get; set; }
}

/// <summary>
/// Identifies why the map view changed.
/// </summary>
public enum MapChangeReason
{
    /// <summary>
    /// The reason is unknown.
    /// </summary>
    Unknown,

    /// <summary>
    /// The view changed because of user interaction.
    /// </summary>
    User,

    /// <summary>
    /// The view changed because of an API call.
    /// </summary>
    Programmatic,

    /// <summary>
    /// The view changed during initial rendering.
    /// </summary>
    Initial,
}

/// <summary>
/// Provides data for marker click events.
/// </summary>
public class MapMarkerEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new marker event args instance.
    /// </summary>
    /// <param name="id">The marker id.</param>
    /// <param name="coordinate">The marker coordinate.</param>
    /// <param name="mouseEventArgs">The map mouse event args.</param>
    public MapMarkerEventArgs( string id, MapCoordinate coordinate, MapMouseEventArgs mouseEventArgs )
    {
        Id = id;
        Coordinate = coordinate;
        MouseEventArgs = mouseEventArgs;
    }

    /// <summary>
    /// Identifies the marker involved in the event.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Contains the marker coordinate.
    /// </summary>
    public MapCoordinate Coordinate { get; }

    /// <summary>
    /// Contains the originating map mouse interaction.
    /// </summary>
    public MapMouseEventArgs MouseEventArgs { get; }
}

/// <summary>
/// Provides data for marker drag events.
/// </summary>
public class MapMarkerDraggedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new marker drag event args instance.
    /// </summary>
    /// <param name="id">The marker id.</param>
    /// <param name="coordinate">The new marker coordinate.</param>
    public MapMarkerDraggedEventArgs( string id, MapCoordinate coordinate )
    {
        Id = id;
        Coordinate = coordinate;
    }

    /// <summary>
    /// Identifies the marker involved in the event.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Contains the marker coordinate after dragging.
    /// </summary>
    public MapCoordinate Coordinate { get; }
}

/// <summary>
/// Provides data for data-bound marker click events.
/// </summary>
/// <typeparam name="TItem">The marker item type.</typeparam>
public class MapMarkerClickedEventArgs<TItem> : EventArgs
{
    /// <summary>
    /// Initializes a new data-bound marker click event args instance.
    /// </summary>
    /// <param name="item">The marker item.</param>
    /// <param name="id">The marker id.</param>
    /// <param name="mouseEventArgs">The map mouse event args.</param>
    public MapMarkerClickedEventArgs( TItem item, string id, MapMouseEventArgs mouseEventArgs )
    {
        Item = item;
        Id = id;
        MouseEventArgs = mouseEventArgs;
    }

    /// <summary>
    /// Contains the data item represented by the marker.
    /// </summary>
    public TItem Item { get; }

    /// <summary>
    /// Identifies the marker involved in the event.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Contains the originating map mouse interaction.
    /// </summary>
    public MapMouseEventArgs MouseEventArgs { get; }
}

/// <summary>
/// Provides data for data-bound marker drag events.
/// </summary>
/// <typeparam name="TItem">The marker item type.</typeparam>
public class MapMarkerDraggedEventArgs<TItem> : EventArgs
{
    /// <summary>
    /// Initializes a new data-bound marker drag event args instance.
    /// </summary>
    /// <param name="item">The marker item.</param>
    /// <param name="id">The marker id.</param>
    /// <param name="coordinate">The new marker coordinate.</param>
    public MapMarkerDraggedEventArgs( TItem item, string id, MapCoordinate coordinate )
    {
        Item = item;
        Id = id;
        Coordinate = coordinate;
    }

    /// <summary>
    /// Contains the data item represented by the marker.
    /// </summary>
    public TItem Item { get; }

    /// <summary>
    /// Identifies the marker involved in the event.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Contains the marker coordinate after dragging.
    /// </summary>
    public MapCoordinate Coordinate { get; }
}