using System.Collections.Generic;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace Blazorise.Tests.Components;

public class DockLayoutSizingTest
{
    [Fact]
    public void MovingSizedPaneOutOfTopTabsPreservesTargetAutoSize()
    {
        DockPane explorer = CreateDockPane( "explorer", DockPanePosition.Left, size: "16rem" );
        DockPane toolbar = CreateDockPane( "toolbar", DockPanePosition.Top );
        DockPane designer = CreateDockPane( "designer", role: DockRole.Document );
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

    [Fact]
    public void RedockingSizedPaneBesideCenterPreservesFixedTrack()
    {
        DockPane explorer = CreateDockPane( "explorer", DockPanePosition.Left, size: "16rem" );
        DockPane designer = CreateDockPane( "designer", role: DockRole.Document );
        DockPane properties = CreateDockPane( "properties", DockPanePosition.Right, size: "18rem" );
        DockLayoutRegistry registry = new();

        registry.RegisterPane( explorer );
        registry.RegisterPane( designer );
        registry.RegisterPane( properties );

        DockLayoutState state = new()
        {
            Panes =
            [
                new() { Name = "explorer", Position = DockPanePosition.Left, Size = "16rem" },
                new() { Name = "designer", Position = DockPanePosition.Center },
                new() { Name = "properties", Position = DockPanePosition.Right, Size = "18rem" },
            ],
        };
        DockLayoutStateManager stateManager = new();
        DockLayoutTreeQuery query = new( registry, stateManager, () => state );
        DockLayoutSizer sizer = new( registry, stateManager, query, () => state );
        DockLayoutTreeMutator mutator = new( query, sizer );
        DockNodeState explorerNode = new() { Kind = DockNodeKind.Pane, PaneName = "explorer" };
        DockNodeState designerNode = new() { Kind = DockNodeKind.Pane, PaneName = "designer" };
        DockNodeState propertiesNode = new() { Kind = DockNodeKind.Pane, PaneName = "properties" };
        DockNodeState centerSplit = DockLayoutTreeBuilder.CreateSplitNode( explorerNode, designerNode, DockSplitOrientation.Horizontal, 0.18 );

        state.Root = DockLayoutTreeBuilder.CreateSplitNode( centerSplit, propertiesNode, DockSplitOrientation.Horizontal, 0.78 );
        state.Panes[0].Position = DockPanePosition.Right;

        mutator.MovePaneToZone( state, "explorer", "properties", null, DockZone.Center, true );

        int nextNodeId = 0;
        stateManager.Normalize( state, registry, query, ref nextNodeId );
        state.Panes[0].Position = DockPanePosition.Left;

        mutator.MovePaneToZone( state, "explorer", "designer", null, DockZone.Left, false );

        DockNodeState redockedSplit = state.Root.First;
        string splitStyle = sizer.GetDockSplitStyle( redockedSplit );

        Assert.False( redockedSplit.UseRatio );
        Assert.Contains( "--dock-split-start-size:16rem", splitStyle );
        Assert.Contains( "--dock-split-end-size:minmax(0,1fr)", splitStyle );
    }

    private static DockPane CreateDockPane( string name, DockPanePosition? position = null, string size = null, DockRole role = DockRole.Tool )
    {
        DockPane pane = new();
        Dictionary<string, object> parameters = new()
        {
            [nameof( DockPane.Name )] = name,
            [nameof( DockPane.Role )] = role,
        };

        if ( position.HasValue )
            parameters[nameof( DockPane.PanePosition )] = position.Value;

        if ( size is not null )
            parameters[nameof( DockPane.Size )] = size;

        ParameterView.FromDictionary( parameters ).SetParameterProperties( pane );

        return pane;
    }
}