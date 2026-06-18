#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Renders a single dock panel inside a dock layout tree.
/// </summary>
public partial class _DockPanelRenderer : BaseComponent
{
    #region Members

    private DockPanel panel;

    private DockPanelState panelState;

    #endregion

    #region Methods

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if ( Layout is null || !Layout.TryGetPanel( PanelName, out panel ) )
            panel = null;

        panelState = Layout?.GetPanelState( PanelName );

        DirtyClasses();
        DirtyStyles();
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        if ( Panel is not null )
        {
            builder.Append( ClassProvider.DockPanel( Panel.EffectivePosition, Panel.Resizable, Collapsed ) );
            builder.Append( ClassProvider.DockPanelPosition( Panel.EffectivePosition ) );
            builder.Append( ClassProvider.DockPanelResizable( Panel.Resizable ) );
            builder.Append( ClassProvider.DockPanelCollapsed( Collapsed ) );
            builder.Append( ClassProvider.DockPanelAutoHide( AutoHide ) );
        }

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        if ( Panel is not null )
        {
            builder.Append( $"--dock-panel-size:{PanelSize}", !AutoHide && !string.IsNullOrWhiteSpace( PanelSize ) );
            builder.Append( $"--dock-panel-min-size:{Panel.MinSize}", !AutoHide && !string.IsNullOrWhiteSpace( Panel.MinSize ) );
            builder.Append( $"--dock-panel-max-size:{Panel.MaxSize}", !AutoHide && !string.IsNullOrWhiteSpace( Panel.MaxSize ) );
        }

        base.BuildStyles( builder );
    }

    private Task OpenAutoHidePanel()
        => Layout?.OpenPanelAutoHide( Panel ) ?? Task.CompletedTask;

    #endregion

    #region Properties

    private DockPanel Panel => panel;

    private bool Visible => Panel is not null && panelState?.Visible != false;

    private bool AutoHide => panelState?.AutoHide == true;

    private bool Collapsed => panelState?.Collapsed == true || AutoHide;

    private string PanelSize => panelState?.Size ?? Panel?.Size;

    private string AutoHideTabClass => ClassProvider.DockPanelAutoHideTab( Panel.EffectivePosition );

    private ElementReference ElementRef
    {
        get => default;
        set
        {
            if ( Panel is not null )
                Panel.ElementRef = value;
        }
    }

    /// <summary>
    /// Gets or sets the owner dock layout.
    /// </summary>
    [Parameter] public DockLayout Layout { get; set; }

    /// <summary>
    /// Gets or sets the panel name.
    /// </summary>
    [Parameter] public string PanelName { get; set; }

    #endregion
}