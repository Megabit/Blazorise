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