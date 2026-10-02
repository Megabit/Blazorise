#region Using directives
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Renders a tabbed dock pane group inside a dock layout tree.
/// </summary>
public partial class _DockTabsRenderer : _BaseDockRenderer
{
    #region Constructors

    /// <summary>
    /// Default <see cref="_DockTabsRenderer"/> constructor.
    /// </summary>
    public _DockTabsRenderer()
    {
        TabsClassBuilder = new( BuildTabsClasses );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.IsParameterChanged( NodeId ) || parameters.IsParameterChanged( Resizable ) )
        {
            DirtyClasses();
            DirtyStyles();
        }

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        if ( ActivePane is not null )
        {
            builder.Append( ClassProvider.DockPane( GroupPosition, CanResize, Collapsed ) );
            builder.Append( ClassProvider.DockPanePosition( GroupPosition ) );
            builder.Append( ClassProvider.DockPaneResizable( CanResize ) );
            builder.Append( ClassProvider.DockPaneCollapsed( Collapsed ) );
            builder.Append( ClassProvider.DockPaneBordered(), Bordered );
            builder.Append( ClassProvider.DockPaneTabsHost() );
        }

        base.BuildClasses( builder );
    }

    private void BuildTabsClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DockPaneTabs() );
        builder.Append( ClassProvider.DockPaneTabsPosition( GroupPosition ) );
        builder.Append( ClassProvider.DockPaneTabPosition( TabPosition ) );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        if ( ActivePane is not null )
        {
            builder.Append( $"{StyleProvider.DockLayoutVariable( "pane-size" )}:{PaneSize}", GroupPosition != DockPanePosition.Center && !string.IsNullOrWhiteSpace( PaneSize ) );
            builder.Append( $"{StyleProvider.DockLayoutVariable( "pane-min-size" )}:{ActivePane.MinSize}", GroupPosition != DockPanePosition.Center && !string.IsNullOrWhiteSpace( ActivePane.MinSize ) );
            builder.Append( $"{StyleProvider.DockLayoutVariable( "pane-max-size" )}:{ActivePane.MaxSize}", GroupPosition != DockPanePosition.Center && !string.IsNullOrWhiteSpace( ActivePane.MaxSize ) );
        }

        base.BuildStyles( builder );
    }

    /// <inheritdoc/>
    protected internal override void DirtyClasses()
    {
        TabsClassBuilder?.Dirty();

        base.DirtyClasses();
    }

    /// <inheritdoc/>
    private protected override bool IsAffected( DockLayoutChange change )
        => change.Kind == DockLayoutChangeKind.Tree
            || change.Kind == DockLayoutChangeKind.Pane && Node?.Panes?.Contains( change.PaneName ) == true
            || change.Kind == DockLayoutChangeKind.Node
                && DockLayoutTreeQuery.FindNodeById( Context?.GetNode( change.NodeId ), NodeId ) is not null;

    /// <inheritdoc/>
    private protected override void OnDockLayoutChanged( DockLayoutChange change )
    {
        DirtyClasses();
        DirtyStyles();
    }

    private string GetTabElementId( string paneName )
        => $"{Context.GetDockNodeElementId( NodeId )}-tab-{Node.Panes.IndexOf( paneName )}";

    #endregion

    #region Properties

    private string ActivePaneName => Context?.GetActiveTabPaneName( Node );

    private string ActiveTabElementId => TabsVisible ? GetTabElementId( ActivePaneName ) : null;

    private DockPane ActivePane
        => !string.IsNullOrWhiteSpace( ActivePaneName ) && Context?.TryGetPane( ActivePaneName, out var pane ) == true ? pane : null;

    private DockPaneState ActivePaneState => string.IsNullOrWhiteSpace( ActivePaneName ) ? null : Context?.GetPaneState( ActivePaneName );

    private bool Visible => Node is not null && ActivePane is not null && ActivePaneState?.Visible != false && ActivePaneState?.AutoHide != true;

    private string TabsClassNames => TabsClassBuilder.Class;

    private bool TabsVisible => Node?.Panes?.Count > 1 || ActivePane?.EffectiveShowTab == true;

    private DockPanePosition GroupPosition
        => string.IsNullOrWhiteSpace( ActivePaneName )
            ? DockPanePosition.Center
            : Context?.GetDockNodePosition( Node ) ?? ActivePane?.EffectivePosition ?? DockPanePosition.Center;

    private DockPaneTabPosition TabPosition
        => string.IsNullOrWhiteSpace( ActivePaneName )
            ? DockPaneTabPosition.Top
            : Context?.GetDockNodeTabPosition( Node, GroupPosition ) ?? DockPaneTabPosition.Top;

    private bool TabsOnTop => TabPosition == DockPaneTabPosition.Top;

    private bool Collapsed => ActivePaneState?.Collapsed == true;

    private string PaneSize => Node?.Size ?? ( ActivePaneState is not null ? ActivePaneState.Size : ActivePane?.Size );

    private bool CanResize => Resizable;

    private bool Bordered => Context?.IsDockPaneBordered( GroupPosition ) == true;

    private ClassBuilder TabsClassBuilder { get; set; }

    private DockNodeState Node => Context?.GetNode( NodeId );

    /// <summary>
    /// Gets or sets the tab node id to render.
    /// </summary>
    [Parameter] public string NodeId { get; set; }

    /// <summary>
    /// Indicates whether the rendered tab group belongs to a resizable split track.
    /// </summary>
    [Parameter] public bool Resizable { get; set; }

    #endregion
}