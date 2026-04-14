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
    #region Members

    private List<( string Id, TItem Item )> materializedItems = new();

    #endregion

    #region Methods

    /// <inheritdoc/>
    internal override MapLayerDefinition ToDefinition()
    {
        materializedItems = new();

        ValidateSelectors();

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

    internal override async ValueTask NotifyDragged( string itemId, MapCoordinate coordinate )
    {
        if ( MarkerDragged.HasDelegate
            && TryFindItem( itemId, out var item ) )
        {
            await MarkerDragged.InvokeAsync( new MapMarkerDraggedEventArgs<TItem>( item, itemId, coordinate ) );
        }
    }

    private MapMarkerDefinition CreateMarker( TItem item )
    {
        var id = IdSelector.Invoke( item );

        if ( string.IsNullOrWhiteSpace( id ) )
            throw new InvalidOperationException( $"The {nameof( IdSelector )} parameter must return a non-empty marker identifier." );

        materializedItems.Add( ( id, item ) );

        return new()
        {
            Id = id,
            Coordinate = CoordinateSelector.Invoke( item ),
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

    private void ValidateSelectors()
    {
        if ( IdSelector is null )
            throw new InvalidOperationException( $"The {nameof( IdSelector )} parameter is required." );

        if ( CoordinateSelector is null )
            throw new InvalidOperationException( $"The {nameof( CoordinateSelector )} parameter is required." );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Provides the data items used to create markers.
    /// </summary>
    [Parameter] public IEnumerable<TItem> Data { get; set; }

    /// <summary>
    /// Selects a stable marker identifier from each data item.
    /// </summary>
    [Parameter, EditorRequired] public Func<TItem, string> IdSelector { get; set; }

    /// <summary>
    /// Selects the marker coordinate from each data item.
    /// </summary>
    [Parameter, EditorRequired] public Func<TItem, MapCoordinate> CoordinateSelector { get; set; }

    /// <summary>
    /// Selects the browser title text for each marker.
    /// </summary>
    [Parameter] public Func<TItem, string> TitleSelector { get; set; }

    /// <summary>
    /// Selects tooltip text for each marker.
    /// </summary>
    [Parameter] public Func<TItem, string> TooltipTextSelector { get; set; }

    /// <summary>
    /// Selects popup text for each marker.
    /// </summary>
    [Parameter] public Func<TItem, string> PopupTextSelector { get; set; }

    /// <summary>
    /// Determines whether each marker can be dragged.
    /// </summary>
    [Parameter] public Func<TItem, bool> DraggableSelector { get; set; }

    /// <summary>
    /// Selects a custom icon for each marker.
    /// </summary>
    [Parameter] public Func<TItem, MapMarkerIcon> IconSelector { get; set; }

    /// <summary>
    /// Handles clicks on data-bound markers.
    /// </summary>
    [Parameter] public EventCallback<MapMarkerClickedEventArgs<TItem>> MarkerClicked { get; set; }

    /// <summary>
    /// Handles completed drag operations for data-bound markers.
    /// </summary>
    [Parameter] public EventCallback<MapMarkerDraggedEventArgs<TItem>> MarkerDragged { get; set; }

    #endregion
}