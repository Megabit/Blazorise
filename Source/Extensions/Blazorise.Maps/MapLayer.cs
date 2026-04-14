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
    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if ( string.IsNullOrWhiteSpace( Id ) )
        {
            Id = Guid.NewGuid().ToString( "N" );
        }

        ParentMap?.RegisterLayer( this );

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override Task OnParametersSetAsync()
    {
        ParentMap?.NotifyLayerChanged( this );

        return base.OnParametersSetAsync();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        ParentMap?.UnregisterLayer( this );
    }

    internal abstract MapLayerDefinition ToDefinition();

    internal virtual ValueTask NotifyClicked( string itemId, MapMouseEventArgs eventArgs )
        => ValueTask.CompletedTask;

    internal virtual ValueTask NotifyDragged( string itemId, MapCoordinate position )
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

    #endregion

    #region Properties

    /// <summary>
    /// Gets the parent map.
    /// </summary>
    [CascadingParameter] protected Map ParentMap { get; set; }

    /// <summary>
    /// Gets or sets the layer identifier.
    /// </summary>
    [Parameter] public string Id { get; set; }

    /// <summary>
    /// Gets or sets the display name for the layer.
    /// </summary>
    [Parameter] public string Name { get; set; }

    /// <summary>
    /// Gets or sets whether the layer is visible.
    /// </summary>
    [Parameter] public bool Visible { get; set; } = true;

    /// <summary>
    /// Gets or sets the layer opacity.
    /// </summary>
    [Parameter] public double Opacity { get; set; } = 1d;

    /// <summary>
    /// Gets or sets the layer z-index.
    /// </summary>
    [Parameter] public int? ZIndex { get; set; }

    /// <summary>
    /// Gets or sets whether the layer participates in pointer interaction.
    /// </summary>
    [Parameter] public bool Interactive { get; set; } = true;

    #endregion
}