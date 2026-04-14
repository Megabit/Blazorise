using System;

namespace Blazorise.Maps;

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