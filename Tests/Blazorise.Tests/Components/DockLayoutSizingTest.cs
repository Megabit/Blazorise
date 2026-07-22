using Blazorise;
using Xunit;

namespace Blazorise.Tests.Components;

public class DockLayoutSizingTest
{
    [Fact]
    public void MovingSizedPaneOutOfTopTabsPreservesTargetAutoSize()
    {
        DockPane explorer = new()
        {
            Name = "explorer",
            PanePosition = DockPanePosition.Left,
            Size = "16rem",
        };
        DockPane toolbar = new()
        {
            Name = "toolbar",
            PanePosition = DockPanePosition.Top,
        };
        DockPane designer = new()
        {
            Name = "designer",
            Role = DockRole.Document,
        };
        DockLayoutRegistry registry = new();

        registry.RegisterPane( explorer );
        registry.RegisterPane( toolbar );
        registry.RegisterPane( designer );

        DockLayoutState state = new()
        {
            Panes =
            [
                new() { Name = "explorer", Position = DockPanePosition.Left, Size = "16rem" },
                new() { Name = "toolbar", Position = DockPanePosition.Top },
                new() { Name = "designer", Position = DockPanePosition.Center },
            ],
        };
        DockLayoutStateManager stateManager = new();
        DockLayoutTreeQuery query = new( registry, stateManager, () => state );
        DockLayoutSizer sizer = new( registry, stateManager, query, () => state );
        DockLayoutTreeMutator mutator = new( query, sizer );
        DockNodeState explorerNode = new() { Id = "explorer-node", Kind = DockNodeKind.Pane, PaneName = "explorer" };
        DockNodeState toolbarNode = new() { Id = "toolbar-node", Kind = DockNodeKind.Pane, PaneName = "toolbar" };
        DockNodeState designerNode = new() { Id = "designer-node", Kind = DockNodeKind.Pane, PaneName = "designer" };
        DockNodeState contentSplit = DockLayoutTreeBuilder.CreateSplitNode( explorerNode, designerNode, DockSplitOrientation.Horizontal, 0.18 );

        contentSplit.Id = "content-split";
        state.Root = DockLayoutTreeBuilder.CreateSplitNode( toolbarNode, contentSplit, DockSplitOrientation.Vertical, 0.12 );
        state.Root.Id = "root-split";
        state.Panes[0].Position = DockPanePosition.Top;

        mutator.MovePaneToZone( state, "explorer", "toolbar", "toolbar-node", DockZone.Center, true );

        DockNodeState topTabs = DockLayoutTreeQuery.FindTabsNode( state.Root, "explorer" );

        Assert.NotNull( topTabs );
        Assert.Equal( "auto", topTabs.Size );

        mutator.MovePaneToZone( state, "explorer", "toolbar", "root-split", DockZone.Top, false );

        int nextNodeId = 0;
        stateManager.Normalize( state, registry, query, ref nextNodeId );

        Assert.Equal( "auto", state.Panes[1].Size );
    }
}