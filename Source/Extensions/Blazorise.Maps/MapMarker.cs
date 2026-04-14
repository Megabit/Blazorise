#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Maps;

/// <summary>
/// Represents a single marker on the map.
/// </summary>
public class MapMarker : MapLayer
{
    /// <inheritdoc/>
    internal override MapLayerDefinition ToDefinition()
    {
        var definition = new MapLayerDefinition
        {
            Kind = MapLayerKind.Marker,
            Markers =
            [
                new()
                {
                    Id = Id,
                    Coordinate = Coordinate,
                    Title = Title,
                    TooltipText = TooltipText,
                    PopupText = PopupText,
                    Draggable = Draggable,
                    Icon = Icon,
                }
            ],
        };

        ApplyBaseDefinition( definition );

        return definition;
    }

    internal override async ValueTask NotifyClicked( string itemId, MapMouseEventArgs eventArgs )
    {
        if ( Clicked.HasDelegate )
        {
            await Clicked.InvokeAsync( new MapMarkerEventArgs( Id, Coordinate, eventArgs ) );
        }
    }

    internal override async ValueTask NotifyDragged( string itemId, MapCoordinate coordinate )
    {
        Coordinate = coordinate;

        if ( Dragged.HasDelegate )
        {
            await Dragged.InvokeAsync( new MapMarkerDraggedEventArgs( Id, coordinate ) );
        }
    }

    /// <summary>
    /// Defines the marker coordinate.
    /// </summary>
    [Parameter] public MapCoordinate Coordinate { get; set; }

    /// <summary>
    /// Provides browser title text for the marker.
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// Provides tooltip text displayed near the marker.
    /// </summary>
    [Parameter] public string TooltipText { get; set; }

    /// <summary>
    /// Provides popup text opened by the marker.
    /// </summary>
    [Parameter] public string PopupText { get; set; }

    /// <summary>
    /// Allows the marker to be moved by dragging.
    /// </summary>
    [Parameter] public bool Draggable { get; set; }

    /// <summary>
    /// Defines a custom marker icon.
    /// </summary>
    [Parameter] public MapMarkerIcon Icon { get; set; }

    /// <summary>
    /// Handles clicks on this marker.
    /// </summary>
    [Parameter] public EventCallback<MapMarkerEventArgs> Clicked { get; set; }

    /// <summary>
    /// Handles completed drag operations for this marker.
    /// </summary>
    [Parameter] public EventCallback<MapMarkerDraggedEventArgs> Dragged { get; set; }
}