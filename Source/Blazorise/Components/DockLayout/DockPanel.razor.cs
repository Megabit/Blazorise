#region Using directives
using System;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// A docked panel that can be placed around the central <see cref="DockContent"/>.
/// </summary>
public partial class DockPanel : BaseComponent, IDisposable
{
    #region Members

    private string name;

    private string caption;

    private DockPanelPosition dock;

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
        builder.Append( ClassProvider.DockPanel( EffectivePosition, Resizable, EffectiveCollapsed || EffectiveAutoHide ) );
        builder.Append( ClassProvider.DockPanelPosition( EffectivePosition ) );
        builder.Append( ClassProvider.DockPanelResizable( Resizable ) );
        builder.Append( ClassProvider.DockPanelCollapsed( EffectiveCollapsed || EffectiveAutoHide ) );
        builder.Append( ClassProvider.DockPanelAutoHide( EffectiveAutoHide ) );
        builder.Append( ClassProvider.DockPanelInactive(), !EffectiveActive );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        builder.Append( $"--dock-panel-size:{EffectiveSize}", !EffectiveAutoHide && !string.IsNullOrWhiteSpace( EffectiveSize ) );
        builder.Append( $"--dock-panel-min-size:{MinSize}", !EffectiveAutoHide && !string.IsNullOrWhiteSpace( MinSize ) );
        builder.Append( $"--dock-panel-max-size:{MaxSize}", !EffectiveAutoHide && !string.IsNullOrWhiteSpace( MaxSize ) );

        base.BuildStyles( builder );
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        ParentDockLayout?.RegisterPanel( this );
        ParentCollector?.AddNode( Node );
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
            ParentDockLayout?.UnregisterPanel( this );

        base.Dispose( disposing );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Identifies the panel inside the parent <see cref="DockLayout"/>.
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
    /// Defines where the panel is docked inside the layout.
    /// </summary>
    [Parameter]
    public DockPanelPosition Dock
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
    /// Allows the panel to be moved to another dock position by dragging its header.
    /// </summary>
    [Parameter]
    public bool Movable
    {
        get => movable;
        set => movable = value;
    }

    /// <summary>
    /// Shows a splitter marker that indicates the panel can participate in resize behavior.
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
    /// Shows or hides the panel in the dock layout.
    /// </summary>
    [Parameter]
    public bool Visible
    {
        get => visible;
        set => visible = value;
    }

    /// <summary>
    /// Collapses the panel content while keeping the panel in the dock layout.
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
    /// Auto-hides the panel content while keeping the panel available on its docked side.
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
    /// Allows the panel header to show a pin action that toggles auto-hide behavior.
    /// </summary>
    [Parameter]
    public bool AutoHideable
    {
        get => autoHideable;
        set => autoHideable = value;
    }

    /// <summary>
    /// Allows the panel header to show a close action that hides the panel.
    /// </summary>
    [Parameter]
    public bool Closable
    {
        get => closable;
        set => closable = value;
    }

    /// <summary>
    /// Defines the preferred panel size, such as <c>280px</c>, <c>18rem</c>, or <c>25%</c>.
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
    /// Defines the minimum panel size when size constraints are applied.
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
    /// Defines the maximum panel size when size constraints are applied.
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
    /// Specifies the content to be rendered inside this <see cref="DockPanel"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    [CascadingParameter] internal DockLayout ParentDockLayout { get; set; }

    [CascadingParameter] internal DockNodeCollector ParentCollector { get; set; }

    internal string ResolvedName => !string.IsNullOrWhiteSpace( Name ) ? Name : ElementId;

    internal string ResolvedCaption => !string.IsNullOrWhiteSpace( Caption ) ? Caption : ResolvedName;

    internal DockPanelPosition EffectivePosition => ParentDockLayout?.GetPanelState( this )?.Position ?? Dock;

    internal string EffectiveSize => ParentDockLayout?.GetPanelState( this )?.Size ?? Size;

    internal bool EffectiveCollapsed => ParentDockLayout?.GetPanelState( this )?.Collapsed ?? Collapsed;

    internal bool EffectiveAutoHide => ParentDockLayout?.GetPanelState( this )?.AutoHide ?? AutoHide;

    internal bool EffectiveVisible => ParentDockLayout?.GetPanelState( this )?.Visible ?? Visible;

    internal bool EffectiveActive => ParentDockLayout?.IsPanelActive( this ) != false;

    internal bool ShowTabs => ParentDockLayout?.HasPanelTabs( EffectivePosition ) == true;

    internal DockNodeState Node
    {
        get
        {
            node ??= new()
            {
                Kind = DockNodeKind.Panel,
            };

            node.PanelName = ResolvedName;

            return node;
        }
    }

    #endregion
}