#region Using directives
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise;

internal sealed class DockLayoutTreeBuilder
{
    #region Members

    private readonly DockLayoutRegistry registry;

    private readonly DockLayoutStateManager stateManager;

    private readonly DockLayoutTreeQuery query;

    private readonly DockLayoutSizer sizer;

    #endregion

    #region Constructors

    public DockLayoutTreeBuilder( DockLayoutRegistry registry, DockLayoutStateManager stateManager, DockLayoutTreeQuery query, DockLayoutSizer sizer )
    {
        this.registry = registry;
        this.stateManager = stateManager;
        this.query = query;
        this.sizer = sizer;
    }

    #endregion

    #region Methods

    public DockNodeState BuildInitialRoot( DockLayoutState state )
    {
        if ( registry.RootCollector.Nodes.Count == 1 && registry.RootCollector.Nodes[0].Kind is DockNodeKind.Split or DockNodeKind.Tabs )
            return registry.RootCollector.Nodes[0];

        DockNodeState center = BuildSimpleDockNode( state, DockPanePosition.Center )
            ?? registry.RootCollector.Nodes.FirstOrDefault( x => x.Kind == DockNodeKind.Content )
            ?? new() { Kind = DockNodeKind.Content };
        DockNodeState left = BuildSimpleDockNode( state, DockPanePosition.Left );
        DockNodeState right = BuildSimpleDockNode( state, DockPanePosition.Right );
        DockNodeState top = BuildSimpleDockNode( state, DockPanePosition.Top );
        DockNodeState bottom = BuildSimpleDockNode( state, DockPanePosition.Bottom );
        DockNodeState root = center;

        if ( left is not null )
            root = CreateSplitNode( left, root, DockSplitOrientation.Horizontal, 0.18 );

        if ( right is not null )
            root = CreateSplitNode( root, right, DockSplitOrientation.Horizontal, 0.78 );

        if ( top is not null )
            root = CreateSplitNode( top, root, DockSplitOrientation.Vertical, 0.12 );

        if ( bottom is not null )
            root = CreateSplitNode( root, bottom, DockSplitOrientation.Vertical, 0.84 );

        return root;
    }

    public static DockNodeState CreateSplitNode( DockNodeState first, DockNodeState second, DockSplitOrientation orientation, double ratio )
        => first is null ? second : second is null ? first : new()
        {
            Kind = DockNodeKind.Split,
            First = first,
            Second = second,
            Orientation = orientation,
            Ratio = ratio,
        };

    private DockNodeState BuildSimpleDockNode( DockLayoutState state, DockPanePosition position )
    {
        List<string> paneNames = registry.RegisteredPanes
            .Where( x => stateManager.EnsurePaneState( state, x )?.Position == position )
            .OrderBy( x => stateManager.FindPaneState( state, x.ResolvedName )?.Order ?? 0 )
            .Select( x => x.ResolvedName )
            .ToList();

        if ( paneNames.Count == 0 )
            return null;

        if ( paneNames.Count == 1 && !query.ShouldKeepSinglePaneTabNode( paneNames[0] ) )
        {
            return new()
            {
                Kind = DockNodeKind.Pane,
                PaneName = paneNames[0],
            };
        }

        return new()
        {
            Kind = DockNodeKind.Tabs,
            Panes = paneNames,
            ActivePane = paneNames[0],
            Size = position == DockPanePosition.Center ? null : sizer.GetDockGroupSize( state, paneNames ),
        };
    }

    #endregion
}