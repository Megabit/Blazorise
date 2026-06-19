#region Using directives
using System;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// A docked pane that can be placed around the central document area.
/// </summary>
public partial class DockPane : BaseComponent, IDisposable
{
    #region Members

    private string name;

    private string caption;

    private DockPanePosition dock;

    private DockRole dockRole = DockRole.Tool;

    private bool showTab = true;

    private bool resizable;

    private bool movable = true;

    private bool collapsed;

    private bool visible = true;

    private bool autoHide;

    private bool autoHideable = true;

    private bool closable = true;

    private string size;

    private string minSize;

    private string maxSize;

    private DockNodeState node;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DockPane( EffectivePosition, Resizable, EffectiveCollapsed || EffectiveAutoHide ) );
        builder.Append( ClassProvider.DockPanePosition( EffectivePosition ) );
        builder.Append( ClassProvider.DockPaneResizable( Resizable ) );
        builder.Append( ClassProvider.DockPaneCollapsed( EffectiveCollapsed || EffectiveAutoHide ) );
        builder.Append( ClassProvider.DockPaneAutoHide( EffectiveAutoHide ) );
        builder.Append( ClassProvider.DockPaneInactive(), !EffectiveActive );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        builder.Append( $"--dock-pane-size:{EffectiveSize}", !EffectiveAutoHide && !string.IsNullOrWhiteSpace( EffectiveSize ) );
        builder.Append( $"--dock-pane-min-size:{MinSize}", !EffectiveAutoHide && !string.IsNullOrWhiteSpace( MinSize ) );
        builder.Append( $"--dock-pane-max-size:{MaxSize}", !EffectiveAutoHide && !string.IsNullOrWhiteSpace( MaxSize ) );

        base.BuildStyles( builder );
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        ParentDockLayout?.RegisterPane( this );
        ParentCollector?.AddNode( Node );
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
            ParentDockLayout?.UnregisterPane( this );

        base.Dispose( disposing );
    }

    #endregion

    #region Properties

    [CascadingParameter] internal DockLayout ParentDockLayout { get; set; }

    [CascadingParameter] internal DockNodeCollector ParentCollector { get; set; }

    internal string ResolvedName => !string.IsNullOrWhiteSpace( Name ) ? Name : ElementId;

    internal string ResolvedCaption => !string.IsNullOrWhiteSpace( Caption ) ? Caption : ResolvedName;

    internal DockPanePosition EffectivePosition => ParentDockLayout?.GetPanePosition( this ) ?? Dock;

    internal string EffectiveSize => ParentDockLayout?.GetPaneState( this )?.Size ?? Size;

    internal bool EffectiveCollapsed => ParentDockLayout?.GetPaneState( this )?.Collapsed ?? Collapsed;

    internal bool EffectiveAutoHide => ParentDockLayout?.GetPaneState( this )?.AutoHide ?? AutoHide;

    internal bool EffectiveVisible => ParentDockLayout?.GetPaneState( this )?.Visible ?? Visible;

    internal bool EffectiveActive => ParentDockLayout?.IsPaneActive( this ) != false;

    internal bool ShowTabs => ParentDockLayout?.HasPaneTabs( EffectivePosition ) == true;

    internal bool EffectiveShowTab => ShowTab;

    internal DockNodeState Node
    {
        get
        {
            node ??= new()
            {
                Kind = DockNodeKind.Pane,
            };

            node.PaneName = ResolvedName;

            return node;
        }
    }

    /// <summary>
    /// Identifies the pane inside the parent <see cref="DockLayout"/>.
    /// </summary>
    [Parameter]
    public string Name
    {
        get => name;
        set => name = value;
    }

    /// <summary>
    /// Defines the caption used by tabbed dock groups.
    /// </summary>
    [Parameter]
    public string Caption
    {
        get => caption;
        set => caption = value;
    }

    /// <summary>
    /// Defines whether this pane behaves as a tool pane or as a document pane.
    /// </summary>
    [Parameter]
    public DockRole DockRole
    {
        get => dockRole;
        set
        {
            if ( dockRole == value )
                return;

            dockRole = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Defines whether this pane should display a tab when it is hosted inside a tab group.
    /// </summary>
    [Parameter]
    public bool ShowTab
    {
        get => showTab;
        set => showTab = value;
    }

    /// <summary>
    /// Defines where the pane is docked inside the layout.
    /// </summary>
    [Parameter]
    public DockPanePosition Dock
    {
        get => dock;
        set
        {
            if ( dock == value )
                return;

            dock = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Allows the pane to be moved to another dock position by dragging its header.
    /// </summary>
    [Parameter]
    public bool Movable
    {
        get => movable;
        set => movable = value;
    }

    /// <summary>
    /// Shows a splitter marker that indicates the pane can participate in resize behavior.
    /// </summary>
    [Parameter]
    public bool Resizable
    {
        get => resizable;
        set
        {
            if ( resizable == value )
                return;

            resizable = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Shows or hides the pane in the dock layout.
    /// </summary>
    [Parameter]
    public bool Visible
    {
        get => visible;
        set => visible = value;
    }

    /// <summary>
    /// Collapses the pane content while keeping the pane in the dock layout.
    /// </summary>
    [Parameter]
    public bool Collapsed
    {
        get => collapsed;
        set
        {
            if ( collapsed == value )
                return;

            collapsed = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Auto-hides the pane content while keeping the pane available on its docked side.
    /// </summary>
    [Parameter]
    public bool AutoHide
    {
        get => autoHide;
        set
        {
            if ( autoHide == value )
                return;

            autoHide = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Allows the pane header to show a pin action that toggles auto-hide behavior.
    /// </summary>
    [Parameter]
    public bool AutoHideable
    {
        get => autoHideable;
        set => autoHideable = value;
    }

    /// <summary>
    /// Allows the pane header to show a close action that hides the pane.
    /// </summary>
    [Parameter]
    public bool Closable
    {
        get => closable;
        set => closable = value;
    }

    /// <summary>
    /// Defines the preferred pane size, such as <c>280px</c>, <c>18rem</c>, or <c>25%</c>.
    /// </summary>
    [Parameter]
    public string Size
    {
        get => size;
        set
        {
            if ( size == value )
                return;

            size = value;

            DirtyStyles();
        }
    }

    /// <summary>
    /// Defines the minimum pane size when size constraints are applied.
    /// </summary>
    [Parameter]
    public string MinSize
    {
        get => minSize;
        set
        {
            if ( minSize == value )
                return;

            minSize = value;

            DirtyStyles();
        }
    }

    /// <summary>
    /// Defines the maximum pane size when size constraints are applied.
    /// </summary>
    [Parameter]
    public string MaxSize
    {
        get => maxSize;
        set
        {
            if ( maxSize == value )
                return;

            maxSize = value;

            DirtyStyles();
        }
    }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="DockPane"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}