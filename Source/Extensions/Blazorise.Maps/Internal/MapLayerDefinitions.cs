using System.Collections.Generic;

namespace Blazorise.Maps;

internal class MapJSOptions
{
    public string Version { get; set; }

    public MapView View { get; set; }

    public MapOptions Options { get; set; }

    public List<MapLayerDefinition> Layers { get; set; }
}

internal class MapLayerDefinition
{
    public string Id { get; set; }

    public string Name { get; set; }

    public MapLayerKind Kind { get; set; }

    public bool Visible { get; set; } = true;

    public double Opacity { get; set; } = 1d;

    public int? ZIndex { get; set; }

    public bool Interactive { get; set; } = true;

    public string Source { get; set; }

    public string Attribution { get; set; }

    public int TileSize { get; set; }

    public double? MinZoom { get; set; }

    public double? MaxZoom { get; set; }

    public string[] Subdomains { get; set; }

    public List<MapMarkerDefinition> Markers { get; set; }

    public MapCoordinate Center { get; set; }

    public double Radius { get; set; }

    public MapShapeStyle Style { get; set; }

    public IReadOnlyList<MapCoordinate> Coordinates { get; set; }

    public IReadOnlyList<IReadOnlyList<MapCoordinate>> Rings { get; set; }
}

internal enum MapLayerKind
{
    Tile,
    Marker,
    Circle,
    Polyline,
    Polygon,
}

internal class MapMarkerDefinition
{
    public string Id { get; set; }

    public MapCoordinate Position { get; set; }

    public string Title { get; set; }

    public string TooltipText { get; set; }

    public string PopupText { get; set; }

    public bool Draggable { get; set; }

    public MapMarkerIcon Icon { get; set; }
}