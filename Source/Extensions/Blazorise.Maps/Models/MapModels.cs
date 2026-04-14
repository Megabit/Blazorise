namespace Blazorise.Maps;

/// <summary>
/// Represents a map coordinate in latitude and longitude.
/// </summary>
/// <param name="Latitude">The latitude.</param>
/// <param name="Longitude">The longitude.</param>
public readonly record struct MapCoordinate( double Latitude, double Longitude );

/// <summary>
/// Represents map bounds.
/// </summary>
/// <param name="SouthWest">The south-west coordinate.</param>
/// <param name="NorthEast">The north-east coordinate.</param>
public readonly record struct MapBounds( MapCoordinate SouthWest, MapCoordinate NorthEast );

/// <summary>
/// Represents a point in screen or container coordinates.
/// </summary>
/// <param name="X">The x coordinate.</param>
/// <param name="Y">The y coordinate.</param>
public readonly record struct MapPoint( double X, double Y );

/// <summary>
/// Represents a size.
/// </summary>
/// <param name="Width">The width.</param>
/// <param name="Height">The height.</param>
public readonly record struct MapSize( double Width, double Height );

/// <summary>
/// Represents the map view.
/// </summary>
public class MapView
{
    /// <summary>
    /// Gets or sets the map center.
    /// </summary>
    public MapCoordinate Center { get; set; }

    /// <summary>
    /// Gets or sets the map zoom level.
    /// </summary>
    public double Zoom { get; set; } = 13d;

    /// <summary>
    /// Gets or sets the visible bounds.
    /// </summary>
    public MapBounds? Bounds { get; set; }
}

/// <summary>
/// Represents shape style.
/// </summary>
public class MapShapeStyle
{
    /// <summary>
    /// Gets or sets the stroke color.
    /// </summary>
    public string StrokeColor { get; set; } = "#3388ff";

    /// <summary>
    /// Gets or sets the stroke opacity.
    /// </summary>
    public double StrokeOpacity { get; set; } = 1d;

    /// <summary>
    /// Gets or sets the stroke width.
    /// </summary>
    public double StrokeWidth { get; set; } = 3d;

    /// <summary>
    /// Gets or sets the stroke dash array.
    /// </summary>
    public string StrokeDashArray { get; set; }

    /// <summary>
    /// Gets or sets the fill color.
    /// </summary>
    public string FillColor { get; set; }

    /// <summary>
    /// Gets or sets the fill opacity.
    /// </summary>
    public double FillOpacity { get; set; } = 0.2d;
}

/// <summary>
/// Represents a marker icon.
/// </summary>
public class MapMarkerIcon
{
    /// <summary>
    /// Gets or sets the icon URL.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Gets or sets the icon size.
    /// </summary>
    public MapSize? Size { get; set; }

    /// <summary>
    /// Gets or sets the icon anchor.
    /// </summary>
    public MapPoint? Anchor { get; set; }

    /// <summary>
    /// Gets or sets a CSS class for the icon.
    /// </summary>
    public string CssClass { get; set; }
}