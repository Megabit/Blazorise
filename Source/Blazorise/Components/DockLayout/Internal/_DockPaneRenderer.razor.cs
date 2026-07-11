#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Renders a single dock pane inside a dock layout tree.
/// </summary>
public partial class _DockPaneRenderer : BaseComponent
{
    #region Members

    private DockPane pane;

    private DockPaneState paneState;

    private DockPanePosition renderPosition;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if ( Context is null || !Context.TryGetPane( PaneName, out pane ) )
            pane = null;

        paneState = Context?.GetPaneState( PaneName );
        renderPosition = Pane is null
            ? DockPanePosition.Center
            : Flyout
                ? GetFlyoutPosition( paneState?.Position ?? Pane.EffectivePosition )
                : Context?.GetPanePosition( Pane ) ?? Pane.EffectivePosition;

        DirtyClasses();
        DirtyStyles();
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
            builder.Append( $"--dock-pane-size:{PaneSize}", RenderPosition != DockPanePosition.Center && !string.IsNullOrWhiteSpace( PaneSize ) );
            builder.Append( $"--dock-pane-min-size:{Pane.MinSize}", RenderPosition != DockPanePosition.Center && !string.IsNullOrWhiteSpace( Pane.MinSize ) );
            builder.Append( $"--dock-pane-max-size:{Pane.MaxSize}", RenderPosition != DockPanePosition.Center && !string.IsNullOrWhiteSpace( Pane.MaxSize ) );
            builder.Append( $"width:{PaneSize}", Flyout && IsHorizontalFlyout && !string.IsNullOrWhiteSpace( PaneSize ) );
            builder.Append( $"height:{PaneSize}", Flyout && IsVerticalFlyout && !string.IsNullOrWhiteSpace( PaneSize ) );
        }

        base.BuildStyles( builder );
    }

    private static DockPanePosition GetFlyoutPosition( DockPanePosition position )
        => position == DockPanePosition.Center ? DockPanePosition.Right : position;

    #endregion

    #region Properties

    private DockPane Pane => pane;

    private bool Visible => Pane is not null && paneState?.Visible != false && ( Flyout || paneState?.AutoHide != true );

    private bool Collapsed => paneState?.Collapsed == true;

    private string PaneSize => paneState?.Size ?? Pane?.Size;

    private bool CanResize => !Flyout && Pane?.Resizable == true && SplitterDock is not null;

    private bool Bordered => Context?.IsDockPaneBordered( RenderPosition ) == true;

    private DockPanePosition RenderPosition => renderPosition;

    private string AutoHideFlyoutPosition => Flyout ? RenderPosition.ToString() : null;

    private bool IsHorizontalFlyout => RenderPosition is DockPanePosition.Left or DockPanePosition.Right;

    private bool IsVerticalFlyout => RenderPosition is DockPanePosition.Top or DockPanePosition.Bottom;

    private ElementReference CapturedElementRef
    {
        get => default;
        set
        {
            if ( Pane is not null )
                Pane.ElementRef = value;
        }
    }

    [CascadingParameter] internal DockLayoutContext Context { get; set; }

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
    /// Gets or sets the local splitter side for the rendered pane.
    /// </summary>
    [Parameter] public DockPanePosition? SplitterDock { get; set; }

    /// <summary>
    /// Gets or sets the split node that owns the rendered pane splitter.
    /// </summary>
    [Parameter] public string SplitNodeId { get; set; }

    #endregion
}