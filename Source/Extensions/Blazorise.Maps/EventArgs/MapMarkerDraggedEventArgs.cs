using System;

namespace Blazorise.Maps;

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