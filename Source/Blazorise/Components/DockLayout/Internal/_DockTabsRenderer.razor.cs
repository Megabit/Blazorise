#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Renders a tabbed dock panel group inside a dock layout tree.
/// </summary>
public partial class _DockTabsRenderer : BaseComponent
{
    #region Members

    private string activePanelName;

    private DockPanel activePanel;

    private DockPanelState activePanelState;

    #endregion

    #region Methods

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        activePanelName = Layout?.GetActiveTabPanelName( Node );

        if ( string.IsNullOrWhiteSpace( activePanelName ) )
        {
            activePanel = null;
            activePanelState = null;
            return;
        }

        if ( Layout is null || !Layout.TryGetPanel( activePanelName, out activePanel ) )
            activePanel = null;

        activePanelState = Layout?.GetPanelState( activePanelName );

        DirtyClasses();
        DirtyStyles();
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        if ( ActivePanel is not null )
        {
            builder.Append( ClassProvider.DockPanel( ActivePanel.EffectivePosition, ActivePanel.Resizable, Collapsed ) );
            builder.Append( ClassProvider.DockPanelPosition( ActivePanel.EffectivePosition ) );
            builder.Append( ClassProvider.DockPanelResizable( ActivePanel.Resizable ) );
            builder.Append( ClassProvider.DockPanelCollapsed( Collapsed ) );
            builder.Append( ClassProvider.DockPanelAutoHide( AutoHide ) );
            builder.Append( "dock-tabs-host" );
        }

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        if ( ActivePanel is not null )
        {
            builder.Append( $"--dock-panel-size:{PanelSize}", !AutoHide && !string.IsNullOrWhiteSpace( PanelSize ) );
            builder.Append( $"--dock-panel-min-size:{ActivePanel.MinSize}", !AutoHide && !string.IsNullOrWhiteSpace( ActivePanel.MinSize ) );
            builder.Append( $"--dock-panel-max-size:{ActivePanel.MaxSize}", !AutoHide && !string.IsNullOrWhiteSpace( ActivePanel.MaxSize ) );
        }

        base.BuildStyles( builder );
    }

    private Task BeginPanelTabDrag( string panelName, PointerEventArgs eventArgs )
        => Layout?.BeginPanelTabDrag( panelName, eventArgs ) ?? Task.CompletedTask;

    private Task ActivateTab( string panelName )
        => Layout?.ActivateTab( Node, panelName ) ?? Task.CompletedTask;

    #endregion

    #region Properties

    private string ActivePanelName => activePanelName;

    private DockPanel ActivePanel => activePanel;

    private bool Visible => Node is not null && ActivePanel is not null && activePanelState?.Visible != false;

    private bool AutoHide => Layout?.IsTabGroupAutoHidden( Node ) == true;

    private string TabsClass => Layout?.GetDockPanelTabsClass();

    private bool Collapsed => activePanelState?.Collapsed == true || AutoHide;

    private string PanelSize => Node?.Size ?? activePanelState?.Size ?? ActivePanel?.Size;

    private ElementReference ElementRef
    {
        get => default;
        set
        {
            if ( ActivePanel is not null )
                ActivePanel.ElementRef = value;
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