namespace Blazorise.Maps;

/// <summary>
/// Represents map interaction options.
/// </summary>
public class MapOptions
{
    /// <summary>
    /// Gets or sets the map provider.
    /// </summary>
    public MapProvider Provider { get; set; } = MapProvider.Leaflet;

    /// <summary>
    /// Gets or sets whether the map is interactive.
    /// </summary>
    public bool Interactive { get; set; } = true;

    /// <summary>
    /// Gets or sets whether dragging is enabled.
    /// </summary>
    public bool Dragging { get; set; } = true;

    /// <summary>
    /// Gets or sets whether scroll wheel zoom is enabled.
    /// </summary>
    public bool ScrollWheelZoom { get; set; } = true;

    /// <summary>
    /// Gets or sets whether double-click zoom is enabled.
    /// </summary>
    public bool DoubleClickZoom { get; set; } = true;

    /// <summary>
    /// Gets or sets whether keyboard navigation is enabled.
    /// </summary>
    public bool Keyboard { get; set; } = true;

    /// <summary>
    /// Gets or sets whether touch zoom is enabled.
    /// </summary>
    public bool TouchZoom { get; set; } = true;

    /// <summary>
    /// Gets or sets the minimum zoom level.
    /// </summary>
    public double? MinZoom { get; set; }

    /// <summary>
    /// Gets or sets the maximum zoom level.
    /// </summary>
    public double? MaxZoom { get; set; }

    /// <summary>
    /// Gets or sets the map control options.
    /// </summary>
    public MapControlOptions Controls { get; set; } = new();
}