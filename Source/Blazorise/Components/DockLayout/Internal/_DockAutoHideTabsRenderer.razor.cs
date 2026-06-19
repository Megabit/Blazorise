#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Renders auto-hidden tabs for a tabbed dock pane group.
/// </summary>
public partial class _DockAutoHideTabsRenderer : BaseComponent
{
    #region Members

    private string activePaneName;

    private DockPane activePane;

    private DockPaneState activePaneState;

    private DockPanePosition groupPosition;

    #endregion

    #region Methods

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        activePaneName = Layout?.GetActiveTabPaneName( Node );

        if ( string.IsNullOrWhiteSpace( activePaneName ) )
        {
            activePane = null;
            activePaneState = null;
            groupPosition = DockPanePosition.Center;
            return;
        }

        if ( Layout is null || !Layout.TryGetPane( activePaneName, out activePane ) )
            activePane = null;

        activePaneState = Layout?.GetPaneState( activePaneName );
        groupPosition = Layout?.GetDockNodePosition( Node ) ?? activePane?.EffectivePosition ?? DockPanePosition.Center;

        DirtyClasses();
        DirtyStyles();
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        if ( ActivePane is not null )
        {
            builder.Append( ClassProvider.DockPane( GroupPosition, ActivePane.Resizable, Collapsed ) );
            builder.Append( ClassProvider.DockPanePosition( GroupPosition ) );
            builder.Append( ClassProvider.DockPaneResizable( ActivePane.Resizable ) );
            builder.Append( ClassProvider.DockPaneCollapsed( Collapsed ) );
            builder.Append( ClassProvider.DockPaneAutoHide( AutoHide ) );
            builder.Append( ClassProvider.DockPaneBordered(), Bordered );
        }

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        if ( ActivePane is not null )
        {
            builder.Append( $"--dock-pane-size:{PaneSize}", !AutoHide && !string.IsNullOrWhiteSpace( PaneSize ) );
            builder.Append( $"--dock-pane-min-size:{ActivePane.MinSize}", !AutoHide && !string.IsNullOrWhiteSpace( ActivePane.MinSize ) );
            builder.Append( $"--dock-pane-max-size:{ActivePane.MaxSize}", !AutoHide && !string.IsNullOrWhiteSpace( ActivePane.MaxSize ) );
        }

        base.BuildStyles( builder );
    }

    private Task OpenPaneAutoHide( DockPane pane )
        => Layout?.OpenPaneAutoHide( pane ) ?? Task.CompletedTask;

    #endregion

    #region Properties

    private string ActivePaneName => activePaneName;

    private DockPane ActivePane => activePane;

    private bool Visible => Node is not null && ActivePane is not null;

    private bool AutoHide => activePaneState?.AutoHide == true;

    private bool Collapsed => activePaneState?.Collapsed == true || AutoHide;

    private string PaneSize => activePaneState?.Size ?? ActivePane?.Size;

    private string AutoHideTabClass => ClassProvider.DockPaneAutoHideTab( GroupPosition );

    private bool Bordered => Layout?.IsDockPaneBordered( GroupPosition ) == true;

    private DockPanePosition GroupPosition => groupPosition;

    private ElementReference ElementRef
    {
        get => default;
        set
        {
            if ( ActivePane is not null )
                ActivePane.ElementRef = value;
        }
    }

    /// <summary>
    /// Gets or sets the owner dock layout.
    /// </summary>
    [Parameter] public DockLayout Layout { get; set; }

    /// <summary>
    /// Gets or sets the tab node to render.
    /// </summary>
    [Parameter] public DockNodeState Node { get; set; }

    #endregion
}