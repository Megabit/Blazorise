#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Maps;

/// <summary>
/// Base class for map layers and features.
/// </summary>
public abstract class MapLayer : ComponentBase, IDisposable
{
    #region Members

    private string registeredId;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        EnsureId();

        registeredId = Id;

        ParentMap?.RegisterLayer( this );

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override Task OnParametersSetAsync()
    {
        EnsureId();

        if ( registeredId is not null
            && !string.Equals( registeredId, Id, StringComparison.Ordinal ) )
        {
            ParentMap?.UnregisterLayer( registeredId );

            registeredId = Id;

            ParentMap?.RegisterLayer( this );
        }
        else
        {
            ParentMap?.NotifyLayerChanged( this );
        }

        return base.OnParametersSetAsync();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        ParentMap?.UnregisterLayer( registeredId ?? Id );
    }

    internal abstract MapLayerDefinition ToDefinition();

    internal virtual ValueTask NotifyClicked( string itemId, MapMouseEventArgs eventArgs )
        => ValueTask.CompletedTask;

    internal virtual ValueTask NotifyDragged( string itemId, MapCoordinate coordinate )
        => ValueTask.CompletedTask;

    private protected void ApplyBaseDefinition( MapLayerDefinition definition )
    {
        definition.Id = Id;
        definition.Name = Name;
        definition.Visible = Visible;
        definition.Opacity = Opacity;
        definition.ZIndex = ZIndex;
        definition.Interactive = Interactive;
    }

    private void EnsureId()
    {
        if ( string.IsNullOrWhiteSpace( Id ) )
        {
            Id = Guid.NewGuid().ToString( "N" );
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Provides the map instance that owns this layer.
    /// </summary>
    [CascadingParameter] protected Map ParentMap { get; set; }

    /// <summary>
    /// Identifies this layer for updates, events, and removal. Changing the value after initialization removes the previous provider layer and registers a new one.
    /// </summary>
    [Parameter] public string Id { get; set; }

    /// <summary>
    /// Provides an optional display name for UI or diagnostics.
    /// </summary>
    [Parameter] public string Name { get; set; }

    /// <summary>
    /// Controls whether this layer is rendered on the map.
    /// </summary>
    [Parameter] public bool Visible { get; set; } = true;

    /// <summary>
    /// Applies layer opacity to tile, marker, and shape rendering, where 1 is fully opaque and 0 is fully transparent.
    /// </summary>
    [Parameter] public double Opacity { get; set; } = 1d;

    /// <summary>
    /// Controls the layer ordering relative to other layers when supported by the provider.
    /// </summary>
    [Parameter] public int? ZIndex { get; set; }

    /// <summary>
    /// Enables pointer interaction for this layer when supported by the provider.
    /// </summary>
    [Parameter] public bool Interactive { get; set; } = true;

    #endregion
}