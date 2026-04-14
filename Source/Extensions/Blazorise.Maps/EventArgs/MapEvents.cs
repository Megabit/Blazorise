using System;

namespace Blazorise.Maps;

/// <summary>
/// Event args for map readiness.
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
    /// Gets the map instance.
    /// </summary>
    public Map Map { get; }
}

/// <summary>
/// Event args for map mouse interaction.
/// </summary>
public class MapMouseEventArgs : EventArgs
{
    /// <summary>
    /// Gets or sets the map coordinate.
    /// </summary>
    public MapCoordinate Coordinate { get; set; }

    /// <summary>
    /// Gets or sets the point within the map container.
    /// </summary>
    public MapPoint ContainerPoint { get; set; }
}

/// <summary>
/// Event args for map view changes.
/// </summary>
public class MapViewChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets or sets the new view.
    /// </summary>
    public MapView View { get; set; }

    /// <summary>
    /// Gets or sets the visible bounds.
    /// </summary>
    public MapBounds Bounds { get; set; }

    /// <summary>
    /// Gets or sets the change reason.
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
/// Event args for a marker click.
/// </summary>
public class MapMarkerEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new marker event args instance.
    /// </summary>
    /// <param name="id">The marker id.</param>
    /// <param name="position">The marker position.</param>
    /// <param name="mouseEventArgs">The map mouse event args.</param>
    public MapMarkerEventArgs( string id, MapCoordinate position, MapMouseEventArgs mouseEventArgs )
    {
        Id = id;
        Position = position;
        MouseEventArgs = mouseEventArgs;
    }

    /// <summary>
    /// Gets the marker id.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the marker position.
    /// </summary>
    public MapCoordinate Position { get; }

    /// <summary>
    /// Gets the map mouse event args.
    /// </summary>
    public MapMouseEventArgs MouseEventArgs { get; }
}

/// <summary>
/// Event args for a marker drag.
/// </summary>
public class MapMarkerDraggedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new marker drag event args instance.
    /// </summary>
    /// <param name="id">The marker id.</param>
    /// <param name="position">The new marker position.</param>
    public MapMarkerDraggedEventArgs( string id, MapCoordinate position )
    {
        Id = id;
        Position = position;
    }

    /// <summary>
    /// Gets the marker id.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the new marker position.
    /// </summary>
    public MapCoordinate Position { get; }
}

/// <summary>
/// Event args for a data-bound marker click.
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
    /// Gets the marker item.
    /// </summary>
    public TItem Item { get; }

    /// <summary>
    /// Gets the marker id.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the map mouse event args.
    /// </summary>
    public MapMouseEventArgs MouseEventArgs { get; }
}

/// <summary>
/// Event args for a data-bound marker drag.
/// </summary>
/// <typeparam name="TItem">The marker item type.</typeparam>
public class MapMarkerDraggedEventArgs<TItem> : EventArgs
{
    /// <summary>
    /// Initializes a new data-bound marker drag event args instance.
    /// </summary>
    /// <param name="item">The marker item.</param>
    /// <param name="id">The marker id.</param>
    /// <param name="position">The new marker position.</param>
    public MapMarkerDraggedEventArgs( TItem item, string id, MapCoordinate position )
    {
        Item = item;
        Id = id;
        Position = position;
    }

    /// <summary>
    /// Gets the marker item.
    /// </summary>
    public TItem Item { get; }

    /// <summary>
    /// Gets the marker id.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the new marker position.
    /// </summary>
    public MapCoordinate Position { get; }
}