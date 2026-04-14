using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Maps;

/// <summary>
/// Represents a polygon shape on the map.
/// </summary>
public class MapPolygon : MapLayer
{
    /// <inheritdoc/>
    internal override MapLayerDefinition ToDefinition()
    {
        var definition = new MapLayerDefinition
        {
            Kind = MapLayerKind.Polygon,
            Rings = Rings,
            Style = Style,
        };

        ApplyBaseDefinition( definition );

        return definition;
    }

    /// <summary>
    /// Gets or sets the polygon rings.
    /// </summary>
    [Parameter] public IReadOnlyList<IReadOnlyList<MapCoordinate>> Rings { get; set; }

    /// <summary>
    /// Gets or sets the polygon style.
    /// </summary>
    [Parameter] public MapShapeStyle Style { get; set; } = new();
}