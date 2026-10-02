#region Using directives
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Renders a single dock pane inside a dock layout tree.
/// </summary>
public partial class _DockPaneRenderer : _BaseDockRenderer
{
    #region Methods

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.IsParameterChanged( PaneName )
            || parameters.IsParameterChanged( NodeId )
            || parameters.IsParameterChanged( Flyout )
            || parameters.IsParameterChanged( Resizable ) )
        {
            DirtyClasses();
            DirtyStyles();
        }

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        if ( Pane is not null )
        {
            builder.Append( ClassProvider.DockPane( RenderPosition, CanResize, Collapsed ) );
            builder.Append( ClassProvider.DockPanePosition( RenderPosition ) );
            builder.Append( ClassProvider.DockPaneResizable( CanResize ) );
            builder.Append( ClassProvider.DockPaneCollapsed( Collapsed ) );
            builder.Append( ClassProvider.DockPaneBordered(), Bordered );
            builder.Append( ClassProvider.DockPaneAutoHideFlyout( RenderPosition ), Flyout );
        }

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        if ( Pane is not null )
        {
            builder.Append( $"{StyleProvider.DockLayoutVariable( "pane-size" )}:{PaneSize}", RenderPosition != DockPanePosition.Center && !string.IsNullOrWhiteSpace( PaneSize ) );
            builder.Append( $"{StyleProvider.DockLayoutVariable( "pane-min-size" )}:{Pane.MinSize}", RenderPosition != DockPanePosition.Center && !string.IsNullOrWhiteSpace( Pane.MinSize ) );
            builder.Append( $"{StyleProvider.DockLayoutVariable( "pane-max-size" )}:{Pane.MaxSize}", RenderPosition != DockPanePosition.Center && !string.IsNullOrWhiteSpace( Pane.MaxSize ) );
            builder.Append( $"width:{PaneSize}", Flyout && IsHorizontalFlyout && !string.IsNullOrWhiteSpace( PaneSize ) );
            builder.Append( $"height:{PaneSize}", Flyout && IsVerticalFlyout && !string.IsNullOrWhiteSpace( PaneSize ) );
        }

        base.BuildStyles( builder );
    }

    /// <inheritdoc/>
    private protected override bool IsAffected( DockLayoutChange change )
        => change.Kind == DockLayoutChangeKind.Tree
            || change.Kind == DockLayoutChangeKind.Pane && change.PaneName == PaneName
            || change.Kind == DockLayoutChangeKind.Node
                && DockLayoutTreeQuery.FindNodeById( Context?.GetNode( change.NodeId ), NodeId ) is not null;

    /// <inheritdoc/>
    private protected override void OnDockLayoutChanged( DockLayoutChange change )
    {
        DirtyClasses();
        DirtyStyles();
    }

    private static DockPanePosition GetFlyoutPosition( DockPanePosition position )
        => position == DockPanePosition.Center ? DockPanePosition.Right : position;

    #endregion

    #region Properties

    private DockPane Pane => Context?.TryGetPane( PaneName, out var pane ) == true ? pane : null;

    private DockPaneState PaneState => Context?.GetPaneState( PaneName );

    private bool Visible => Pane is not null && PaneState?.Visible != false && ( Flyout || PaneState?.AutoHide != true );

    private bool Collapsed => PaneState?.Collapsed == true;

    private string PaneSize => PaneState is not null ? PaneState.Size : Pane?.Size;

    private bool CanResize => !Flyout && Resizable;

    private bool Bordered => Context?.IsDockPaneBordered( RenderPosition ) == true;

    private DockPanePosition RenderPosition
        => Pane is null
            ? DockPanePosition.Center
            : Flyout
                ? GetFlyoutPosition( PaneState?.Position ?? Pane.EffectivePosition )
                : Context?.GetPanePosition( Pane ) ?? Pane.EffectivePosition;

    private string AutoHideFlyoutPosition => Flyout ? RenderPosition.ToString() : null;

    private bool IsHorizontalFlyout => RenderPosition is DockPanePosition.Left or DockPanePosition.Right;

    private bool IsVerticalFlyout => RenderPosition is DockPanePosition.Top or DockPanePosition.Bottom;

    /// <summary>
    /// Gets or sets the pane name.
    /// </summary>
    [Parameter] public string PaneName { get; set; }

    /// <summary>
    /// Gets or sets the rendered dock node id.
    /// </summary>
    [Parameter] public string NodeId { get; set; }

    /// <summary>
    /// Gets or sets whether the pane is rendered as a temporary auto-hide flyout.
    /// </summary>
    [Parameter] public bool Flyout { get; set; }

    /// <summary>
    /// Indicates whether the rendered pane belongs to a resizable split track.
    /// </summary>
    [Parameter] public bool Resizable { get; set; }

    #endregion
}