using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Maps;

/// <summary>
/// Represents a polyline shape on the map.
/// </summary>
public class MapPolyline : MapLayer
{
    #region Methods

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

    #endregion

    #region Properties

    /// <summary>
    /// Specifies the ordered coordinates connected by the polyline.
    /// </summary>
    [Parameter] public IReadOnlyList<MapCoordinate> Coordinates { get; set; }

    /// <summary>
    /// Controls the polyline stroke styling.
    /// </summary>
    [Parameter] public MapShapeStyle Style { get; set; } = new();

    #endregion
}