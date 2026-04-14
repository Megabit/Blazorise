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
                    Position = Position,
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
            await Clicked.InvokeAsync( new MapMarkerEventArgs( Id, Position, eventArgs ) );
        }
    }

    internal override async ValueTask NotifyDragged( string itemId, MapCoordinate position )
    {
        Position = position;

        if ( Dragged.HasDelegate )
        {
            await Dragged.InvokeAsync( new MapMarkerDraggedEventArgs( Id, position ) );
        }
    }

    /// <summary>
    /// Gets or sets the marker position.
    /// </summary>
    [Parameter] public MapCoordinate Position { get; set; }

    /// <summary>
    /// Gets or sets the marker title.
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// Gets or sets the marker tooltip text.
    /// </summary>
    [Parameter] public string TooltipText { get; set; }

    /// <summary>
    /// Gets or sets the marker popup text.
    /// </summary>
    [Parameter] public string PopupText { get; set; }

    /// <summary>
    /// Gets or sets whether the marker can be dragged.
    /// </summary>
    [Parameter] public bool Draggable { get; set; }

    /// <summary>
    /// Gets or sets the marker icon.
    /// </summary>
    [Parameter] public MapMarkerIcon Icon { get; set; }

    /// <summary>
    /// Occurs when the marker is clicked.
    /// </summary>
    [Parameter] public EventCallback<MapMarkerEventArgs> Clicked { get; set; }

    /// <summary>
    /// Occurs when the marker is dragged.
    /// </summary>
    [Parameter] public EventCallback<MapMarkerDraggedEventArgs> Dragged { get; set; }
}