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
/// Describes the map camera state.
/// </summary>
public class MapView
{
    /// <summary>
    /// Defines the coordinate at the center of the map.
    /// </summary>
    public MapCoordinate Center { get; set; }

    /// <summary>
    /// Defines the map zoom level.
    /// </summary>
    public double Zoom { get; set; } = 13d;

    /// <summary>
    /// Stores the visible bounds reported by the map provider.
    /// </summary>
    public MapBounds? Bounds { get; set; }
}

/// <summary>
/// Describes stroke and fill styling for map shapes.
/// </summary>
public class MapShapeStyle
{
    /// <summary>
    /// Defines the stroke color.
    /// </summary>
    public string StrokeColor { get; set; } = "#3388ff";

    /// <summary>
    /// Controls stroke opacity.
    /// </summary>
    public double StrokeOpacity { get; set; } = 1d;

    /// <summary>
    /// Defines stroke width in pixels.
    /// </summary>
    public double StrokeWidth { get; set; } = 3d;

    /// <summary>
    /// Defines the stroke dash pattern.
    /// </summary>
    public string StrokeDashArray { get; set; }

    /// <summary>
    /// Defines the fill color.
    /// </summary>
    public string FillColor { get; set; }

    /// <summary>
    /// Controls fill opacity.
    /// </summary>
    public double FillOpacity { get; set; } = 0.2d;
}

/// <summary>
/// Describes a custom marker icon.
/// </summary>
public class MapMarkerIcon
{
    /// <summary>
    /// Provides the icon image source.
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// Defines the icon image size.
    /// </summary>
    public MapSize? Size { get; set; }

    /// <summary>
    /// Defines the pixel point on the icon that anchors to the marker coordinate.
    /// </summary>
    public MapPoint? Anchor { get; set; }

    /// <summary>
    /// Applies a CSS class to the icon element.
    /// </summary>
    public string CssClass { get; set; }
}