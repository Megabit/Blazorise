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
    /// Enables or disables user interaction with the map surface and child layers.
    /// </summary>
    public bool Interactive { get; set; } = true;

    /// <summary>
    /// Allows users to pan the map by dragging.
    /// </summary>
    public bool Panning { get; set; } = true;

    /// <summary>
    /// Allows users to zoom with the mouse wheel or trackpad.
    /// </summary>
    public bool ZoomOnScroll { get; set; } = true;

    /// <summary>
    /// Allows users to zoom by double-clicking the map.
    /// </summary>
    public bool ZoomOnDoubleClick { get; set; } = true;

    /// <summary>
    /// Allows keyboard navigation when the map has focus.
    /// </summary>
    public bool KeyboardNavigation { get; set; } = true;

    /// <summary>
    /// Allows touch and pinch zoom gestures.
    /// </summary>
    public bool ZoomOnTouch { get; set; } = true;

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