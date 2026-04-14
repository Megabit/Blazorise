using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Maps;

/// <summary>
/// Represents a polyline shape on the map.
/// </summary>
public class MapPolyline : MapLayer
{
    /// <inheritdoc/>
    internal override MapLayerDefinition ToDefinition()
    {
        var definition = new MapLayerDefinition
        {
            Kind = MapLayerKind.Polyline,
            Coordinates = Coordinates,
            Style = Style,
        };

        ApplyBaseDefinition( definition );

        return definition;
    }

    /// <summary>
    /// Gets or sets the polyline coordinates.
    /// </summary>
    [Parameter] public IReadOnlyList<MapCoordinate> Coordinates { get; set; }

    /// <summary>
    /// Gets or sets the polyline style.
    /// </summary>
    [Parameter] public MapShapeStyle Style { get; set; } = new();
}