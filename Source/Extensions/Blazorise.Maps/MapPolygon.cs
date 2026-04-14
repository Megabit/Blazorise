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
    /// Defines the polygon rings. The first ring is the outer boundary; additional rings can represent holes.
    /// </summary>
    [Parameter] public IReadOnlyList<IReadOnlyList<MapCoordinate>> Rings { get; set; }

    /// <summary>
    /// Controls the polygon stroke and fill styling.
    /// </summary>
    [Parameter] public MapShapeStyle Style { get; set; } = new();
}