#region Using directives
using System.Threading.Tasks;
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

    #endregion

    #region Methods

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if ( Layout is null || !Layout.TryGetPane( PaneName, out pane ) )
            pane = null;

        paneState = Layout?.GetPaneState( PaneName );

        DirtyClasses();
        DirtyStyles();
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        if ( Pane is not null )
        {
            builder.Append( ClassProvider.DockPane( Pane.EffectivePosition, CanResize, Collapsed ) );
            builder.Append( ClassProvider.DockPanePosition( Pane.EffectivePosition ) );
            builder.Append( ClassProvider.DockPaneResizable( CanResize ) );
            builder.Append( ClassProvider.DockPaneCollapsed( Collapsed ) );
            builder.Append( ClassProvider.DockPaneAutoHide( AutoHide ) );
        }

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        if ( Pane is not null )
        {
            builder.Append( $"--dock-pane-size:{PaneSize}", !AutoHide && !string.IsNullOrWhiteSpace( PaneSize ) );
            builder.Append( $"--dock-pane-min-size:{Pane.MinSize}", !AutoHide && !string.IsNullOrWhiteSpace( Pane.MinSize ) );
            builder.Append( $"--dock-pane-max-size:{Pane.MaxSize}", !AutoHide && !string.IsNullOrWhiteSpace( Pane.MaxSize ) );
        }

        base.BuildStyles( builder );
    }

    private Task OpenAutoHidePane()
        => Layout?.OpenPaneAutoHide( Pane ) ?? Task.CompletedTask;

    #endregion

    #region Properties

    private DockPane Pane => pane;

    private bool Visible => Pane is not null && paneState?.Visible != false;

    private bool AutoHide => paneState?.AutoHide == true;

    private bool Collapsed => paneState?.Collapsed == true || AutoHide;

    private string PaneSize => paneState?.Size ?? Pane?.Size;

    private string AutoHideTabClass => ClassProvider.DockPaneAutoHideTab( Pane.EffectivePosition );

    private bool CanResize => Pane?.Resizable == true && Pane.EffectivePosition != DockPanePosition.Center;

    private ElementReference ElementRef
    {
        get => default;
        set
        {
            if ( Pane is not null )
                Pane.ElementRef = value;
        }
    }

    /// <summary>
    /// Gets or sets the owner dock layout.
    /// </summary>
    [Parameter] public DockLayout Layout { get; set; }

    /// <summary>
    /// Gets or sets the pane name.
    /// </summary>
    [Parameter] public string PaneName { get; set; }

    #endregion
}