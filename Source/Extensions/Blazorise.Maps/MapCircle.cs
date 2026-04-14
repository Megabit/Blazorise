using Microsoft.AspNetCore.Components;

namespace Blazorise.Maps;

/// <summary>
/// Represents a circle shape on the map.
/// </summary>
public class MapCircle : MapLayer
{
    /// <inheritdoc/>
    internal override MapLayerDefinition ToDefinition()
    {
        var definition = new MapLayerDefinition
        {
            Kind = MapLayerKind.Circle,
            Center = Center,
            Radius = Radius,
            Style = Style,
        };

        ApplyBaseDefinition( definition );

        return definition;
    }

    /// <summary>
    /// Gets or sets the circle center.
    /// </summary>
    [Parameter] public MapCoordinate Center { get; set; }

    /// <summary>
    /// Gets or sets the circle radius in meters.
    /// </summary>
    [Parameter] public double Radius { get; set; }

    /// <summary>
    /// Gets or sets the circle style.
    /// </summary>
    [Parameter] public MapShapeStyle Style { get; set; } = new();
}