using System;

namespace Blazorise.Maps;

/// <summary>
/// Provides data for map view updates.
/// </summary>
public class MapViewUpdatedEventArgs : EventArgs
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