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
    /// Gets or sets the tile URL template.
    /// </summary>
    [Parameter, EditorRequired] public string UrlTemplate { get; set; }

    /// <summary>
    /// Gets or sets the attribution text required by the tile provider.
    /// </summary>
    [Parameter] public string Attribution { get; set; }

    /// <summary>
    /// Gets or sets the tile size in pixels.
    /// </summary>
    [Parameter] public int TileSize { get; set; } = 256;

    /// <summary>
    /// Gets or sets the minimum zoom level for this layer.
    /// </summary>
    [Parameter] public double? MinZoom { get; set; }

    /// <summary>
    /// Gets or sets the maximum zoom level for this layer.
    /// </summary>
    [Parameter] public double? MaxZoom { get; set; }

    /// <summary>
    /// Gets or sets tile subdomains.
    /// </summary>
    [Parameter] public string[] Subdomains { get; set; }
}