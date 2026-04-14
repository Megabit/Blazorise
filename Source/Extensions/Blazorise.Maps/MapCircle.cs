using Microsoft.AspNetCore.Components;

namespace Blazorise.Maps;

/// <summary>
/// Represents a circle shape on the map.
/// </summary>
public class MapCircle : MapLayer
{
    #region Methods

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

    #endregion

    #region Properties

    /// <summary>
    /// Defines the circle center coordinate.
    /// </summary>
    [Parameter] public MapCoordinate Center { get; set; }

    /// <summary>
    /// Defines the circle radius in meters.
    /// </summary>
    [Parameter] public double Radius { get; set; }

    /// <summary>
    /// Controls the circle stroke and fill styling.
    /// </summary>
    [Parameter] public MapShapeStyle Style { get; set; } = new();

    #endregion
}