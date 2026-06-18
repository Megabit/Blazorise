#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Renders auto-hidden tabs for a tabbed dock panel group.
/// </summary>
public partial class _DockAutoHideTabsRenderer : BaseComponent
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

    private Task OpenPanelAutoHide( DockPanel panel )
        => Layout?.OpenPanelAutoHide( panel ) ?? Task.CompletedTask;

    private string GetPanelAutoHideTabClass( DockPanel panel )
        => ClassProvider.DockPanelAutoHideTab( panel.EffectivePosition );

    #endregion

    #region Properties

    private string ActivePanelName => activePanelName;

    private DockPanel ActivePanel => activePanel;

    private bool Visible => Node is not null && ActivePanel is not null;

    private bool AutoHide => activePanelState?.AutoHide == true;

    private bool Collapsed => activePanelState?.Collapsed == true || AutoHide;

    private string PanelSize => activePanelState?.Size ?? ActivePanel?.Size;

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