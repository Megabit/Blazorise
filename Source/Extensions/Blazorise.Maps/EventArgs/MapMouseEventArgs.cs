using System;

namespace Blazorise.Maps;

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