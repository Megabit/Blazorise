using Microsoft.AspNetCore.Components;

namespace Blazorise.Maps;

/// <summary>
/// Represents a raster tile layer.
/// </summary>
public class MapTileLayer : MapLayer
{
    /// <inheritdoc/>
    internal override MapLayerDefinition ToDefinition()
    {
        var definition = new MapLayerDefinition
        {
            Kind = MapLayerKind.Tile,
            UrlTemplate = UrlTemplate,
            Attribution = Attribution,
            TileSize = TileSize,
            MinZoom = MinZoom,
            MaxZoom = MaxZoom,
            Subdomains = Subdomains,
        };

        ApplyBaseDefinition( definition );

        return definition;
    }

    /// <summary>
    /// Defines the raster tile URL template, typically using {z}, {x}, and {y} placeholders.
    /// </summary>
    [Parameter, EditorRequired] public string UrlTemplate { get; set; }

    /// <summary>
    /// Provides attribution text required by the tile provider.
    /// </summary>
    [Parameter] public string Attribution { get; set; }

    /// <summary>
    /// Defines the tile size in pixels.
    /// </summary>
    [Parameter] public int TileSize { get; set; } = 256;

    /// <summary>
    /// Limits this tile layer to the specified minimum zoom level.
    /// </summary>
    [Parameter] public double? MinZoom { get; set; }

    /// <summary>
    /// Limits this tile layer to the specified maximum zoom level.
    /// </summary>
    [Parameter] public double? MaxZoom { get; set; }

    /// <summary>
    /// Defines optional tile subdomains used by the URL template.
    /// </summary>
    [Parameter] public string[] Subdomains { get; set; }
}