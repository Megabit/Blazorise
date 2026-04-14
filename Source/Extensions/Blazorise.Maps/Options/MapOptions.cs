namespace Blazorise.Maps;

/// <summary>
/// Configures map provider, interactions, zoom limits, and controls.
/// </summary>
public class MapOptions
{
    /// <summary>
    /// Selects the map provider implementation.
    /// </summary>
    public MapProvider Provider { get; set; } = MapProvider.Leaflet;

    /// <summary>
    /// Enables or disables map interaction globally.
    /// </summary>
    public bool Interactive { get; set; } = true;

    /// <summary>
    /// Allows users to pan the map by dragging.
    /// </summary>
    public bool Dragging { get; set; } = true;

    /// <summary>
    /// Allows users to zoom with the mouse wheel or trackpad.
    /// </summary>
    public bool ScrollWheelZoom { get; set; } = true;

    /// <summary>
    /// Allows users to zoom by double-clicking the map.
    /// </summary>
    public bool DoubleClickZoom { get; set; } = true;

    /// <summary>
    /// Allows keyboard navigation when the map has focus.
    /// </summary>
    public bool Keyboard { get; set; } = true;

    /// <summary>
    /// Allows touch and pinch zoom gestures.
    /// </summary>
    public bool TouchZoom { get; set; } = true;

    /// <summary>
    /// Defines the minimum allowed map zoom level.
    /// </summary>
    public double? MinZoom { get; set; }

    /// <summary>
    /// Defines the maximum allowed map zoom level.
    /// </summary>
    public double? MaxZoom { get; set; }

    /// <summary>
    /// Configures built-in map controls.
    /// </summary>
    public MapControlOptions Controls { get; set; } = new();
}