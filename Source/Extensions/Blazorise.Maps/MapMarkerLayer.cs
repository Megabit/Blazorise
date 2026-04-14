#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Maps;

/// <summary>
/// Represents a data-bound marker layer.
/// </summary>
/// <typeparam name="TItem">The marker item type.</typeparam>
public class MapMarkerLayer<TItem> : MapLayer
{
    private List<( string Id, TItem Item )> materializedItems = new();

    /// <inheritdoc/>
    internal override MapLayerDefinition ToDefinition()
    {
        materializedItems = new();

        var markers = Data?.Select( CreateMarker ).ToList() ?? [];

        var definition = new MapLayerDefinition
        {
            Kind = MapLayerKind.Marker,
            Markers = markers,
        };

        ApplyBaseDefinition( definition );

        return definition;
    }

    internal override async ValueTask NotifyClicked( string itemId, MapMouseEventArgs eventArgs )
    {
        if ( MarkerClicked.HasDelegate
            && TryFindItem( itemId, out var item ) )
        {
            await MarkerClicked.InvokeAsync( new MapMarkerClickedEventArgs<TItem>( item, itemId, eventArgs ) );
        }
    }

    internal override async ValueTask NotifyDragged( string itemId, MapCoordinate position )
    {
        if ( MarkerDragged.HasDelegate
            && TryFindItem( itemId, out var item ) )
        {
            await MarkerDragged.InvokeAsync( new MapMarkerDraggedEventArgs<TItem>( item, itemId, position ) );
        }
    }

    private MapMarkerDefinition CreateMarker( TItem item )
    {
        var id = IdSelector?.Invoke( item ) ?? Guid.NewGuid().ToString( "N" );

        materializedItems.Add( ( id, item ) );

        return new()
        {
            Id = id,
            Position = PositionSelector is not null ? PositionSelector.Invoke( item ) : default,
            Title = TitleSelector?.Invoke( item ),
            TooltipText = TooltipTextSelector?.Invoke( item ),
            PopupText = PopupTextSelector?.Invoke( item ),
            Draggable = DraggableSelector?.Invoke( item ) ?? false,
            Icon = IconSelector?.Invoke( item ),
        };
    }

    private bool TryFindItem( string itemId, out TItem item )
    {
        foreach ( var materializedItem in materializedItems )
        {
            if ( string.Equals( materializedItem.Id, itemId, StringComparison.Ordinal ) )
            {
                item = materializedItem.Item;
                return true;
            }
        }

        item = default;
        return false;
    }

    /// <summary>
    /// Gets or sets the layer data.
    /// </summary>
    [Parameter] public IEnumerable<TItem> Data { get; set; }

    /// <summary>
    /// Gets or sets the marker id selector.
    /// </summary>
    [Parameter] public Func<TItem, string> IdSelector { get; set; }

    /// <summary>
    /// Gets or sets the marker position selector.
    /// </summary>
    [Parameter, EditorRequired] public Func<TItem, MapCoordinate> PositionSelector { get; set; }

    /// <summary>
    /// Gets or sets the marker title selector.
    /// </summary>
    [Parameter] public Func<TItem, string> TitleSelector { get; set; }

    /// <summary>
    /// Gets or sets the marker tooltip text selector.
    /// </summary>
    [Parameter] public Func<TItem, string> TooltipTextSelector { get; set; }

    /// <summary>
    /// Gets or sets the marker popup text selector.
    /// </summary>
    [Parameter] public Func<TItem, string> PopupTextSelector { get; set; }

    /// <summary>
    /// Gets or sets the marker draggable selector.
    /// </summary>
    [Parameter] public Func<TItem, bool> DraggableSelector { get; set; }

    /// <summary>
    /// Gets or sets the marker icon selector.
    /// </summary>
    [Parameter] public Func<TItem, MapMarkerIcon> IconSelector { get; set; }

    /// <summary>
    /// Occurs when a marker is clicked.
    /// </summary>
    [Parameter] public EventCallback<MapMarkerClickedEventArgs<TItem>> MarkerClicked { get; set; }

    /// <summary>
    /// Occurs when a marker is dragged.
    /// </summary>
    [Parameter] public EventCallback<MapMarkerDraggedEventArgs<TItem>> MarkerDragged { get; set; }
}