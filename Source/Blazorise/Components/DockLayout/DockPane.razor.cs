#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// A docked pane that can be placed around the central document area.
/// </summary>
public partial class DockPane : BaseComponent, IDisposable
{
    #region Members

    private DockNodeState node;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        ParentDockLayout?.RegisterPane( this );
        Node.PaneName = ResolvedName;
        ParentCollector?.AddNode( Node );
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
            ParentDockLayout?.UnregisterPane( this );

        base.Dispose( disposing );
    }

    /// <summary>
    /// Forces rendered pane content to refresh without changing the docking state.
    /// </summary>
    /// <returns>A task that completes after the refresh has been scheduled.</returns>
    public Task Refresh()
        => ParentDockLayout is null
            ? InvokeAsync( StateHasChanged )
            : ParentDockLayout.Refresh();

    #endregion

    #region Properties

    internal string ResolvedName => !string.IsNullOrWhiteSpace( Name ) ? Name : ElementId;

    internal string ResolvedCaption => !string.IsNullOrWhiteSpace( Caption ) ? Caption : ResolvedName;

    internal DockPanePosition EffectivePosition => ParentDockLayout?.GetPanePosition( this ) ?? PanePosition;

    internal bool EffectiveAutoHide => ParentDockLayout?.GetPaneState( this )?.AutoHide ?? AutoHide;

    internal bool EffectiveShowTab => ShowTab;

    internal bool EffectiveShowTabCloseButton => ShowTabCloseButton;

    internal DockPaneTabPosition EffectiveTabPosition => TabPosition;

    internal DockNodeState Node => node ??= new()
    {
        Kind = DockNodeKind.Pane,
    };

    [CascadingParameter] internal DockLayout ParentDockLayout { get; set; }

    [CascadingParameter] internal DockNodeCollector ParentCollector { get; set; }

    /// <summary>
    /// Identifies the pane inside the parent <see cref="DockLayout"/> and acts as the stable key used by persisted <see cref="DockLayoutState"/> values.
    /// </summary>
    [Parameter] public string Name { get; set; }

    /// <summary>
    /// Defines the caption used by tabbed dock groups.
    /// </summary>
    [Parameter] public string Caption { get; set; }

    /// <summary>
    /// Defines whether this pane behaves as a tool pane or as a document pane.
    /// </summary>
    [Parameter] public DockRole Role { get; set; } = DockRole.Tool;

    /// <summary>
    /// Defines whether this pane should display a tab when it is hosted inside a tab group.
    /// </summary>
    [Parameter] public bool ShowTab { get; set; } = true;

    /// <summary>
    /// Defines whether a close button should be shown when this pane is rendered as a document tab.
    /// </summary>
    [Parameter] public bool ShowTabCloseButton { get; set; }

    /// <summary>
    /// Defines where tabs should be displayed when this pane is hosted inside a tab group.
    /// </summary>
    [Parameter] public DockPaneTabPosition TabPosition { get; set; }

    /// <summary>
    /// Defines where the pane is docked inside the layout.
    /// </summary>
    [Parameter] public DockPanePosition PanePosition { get; set; }

    /// <summary>
    /// Allows the pane to be moved to another dock position by dragging its header.
    /// </summary>
    [Parameter] public bool Movable { get; set; } = true;

    /// <summary>
    /// Shows a splitter marker that indicates the pane can participate in resize behavior.
    /// </summary>
    [Parameter] public bool Resizable { get; set; }

    /// <summary>
    /// Shows or hides the pane in the dock layout.
    /// </summary>
    [Parameter] public bool Visible { get; set; } = true;

    /// <summary>
    /// Collapses the pane content while keeping the pane in the dock layout.
    /// </summary>
    [Parameter] public bool Collapsed { get; set; }

    /// <summary>
    /// Auto-hides the pane content while keeping the pane available on its docked side.
    /// </summary>
    [Parameter] public bool AutoHide { get; set; }

    /// <summary>
    /// Allows the pane header to show a pin action that toggles auto-hide behavior.
    /// </summary>
    [Parameter] public bool AutoHideable { get; set; } = true;

    /// <summary>
    /// Allows the pane header to show a close action that hides the pane.
    /// </summary>
    [Parameter] public bool Closable { get; set; } = true;

    /// <summary>
    /// Defines the preferred pane size, such as <c>280px</c>, <c>18rem</c>, or <c>25%</c>.
    /// </summary>
    [Parameter] public string Size { get; set; }

    /// <summary>
    /// Defines the minimum pane size when size constraints are applied.
    /// </summary>
    [Parameter] public string MinSize { get; set; }

    /// <summary>
    /// Defines the maximum pane size when size constraints are applied.
    /// </summary>
    [Parameter] public string MaxSize { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="DockPane"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}