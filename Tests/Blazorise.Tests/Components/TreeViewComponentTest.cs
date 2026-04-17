#region Using directives
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.TreeView;
using Blazorise.TreeView.EventArguments;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
using static BasicTestApp.Client.TreeViewComponent;
#endregion

namespace Blazorise.Tests.Components;

public class TreeViewComponentTest : TestContext
{
    public TreeViewComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseButton();
    }

    [Fact]
    public async Task Items_Add_Should_Update_UI_WhenObservable()
    {
        // setup
        var items = new ObservableCollection<Item>() {
            new Item()
            {
                Text = "Item 1"
            },
            new Item()
            {
                Text = "Item 2"
            }
        };

        var cut = RenderComponent<TreeViewComponent>( parameters =>
        {
            parameters.Add( x => x.Items, items );
        } );

        await cut.ClickOnAsync( "#btnAdd" );

        var nodes = cut.FindAll( ".b-tree-view .b-tree-view-node .b-tree-view-node-title" );
        cut.WaitForAssertion( () =>
        {
            nodes.Refresh();
            nodes.Count.Should().Be( 3 );
            nodes[2].TextContent.Should().Contain( "Item 3" );
        } );
    }

    [Fact]
    public async Task Items_Remove_Should_Update_UI_WhenObservable()
    {
        // setup
        var items = new ObservableCollection<Item>() {
            new Item()
            {
                Text = "Item 1"
            },
            new Item()
            {
                Text = "Item 2"
            }
        };

        var cut = RenderComponent<TreeViewComponent>( parameters =>
        {
            parameters.Add( x => x.Items, items );
        } );

        await cut.ClickOnAsync( "#btnRemove" );

        var nodes = cut.FindAll( ".b-tree-view .b-tree-view-node .b-tree-view-node-title" );
        cut.WaitForAssertion( () =>
        {
            nodes.Refresh();
            nodes.Count.Should().Be( 1 );
        } );
    }

    [Fact]
    public void Items_Mutate_Should_Update_UI_WhenObservable()
    {
        // setup
        var items = new ObservableCollection<Item>() {
            new Item()
            {
                Text = "Item 1"
            },
            new Item()
            {
                Text = "Item 2"
            }
        };

        var cut = RenderComponent<TreeViewComponent>( parameters =>
        {
            parameters.Add( x => x.Items, items );
        } );

        items[0] = new Item()
        {
            Text = "Item Mutated"
        };

        cut.Render();

        var nodes = cut.FindAll( ".b-tree-view .b-tree-view-node .b-tree-view-node-title" );
        cut.WaitForAssertion( () =>
        {
            nodes.Refresh();
            nodes.Count.Should().Be( 2 );
            nodes[0].TextContent.Should().Contain( "Item Mutated" );
        } );
    }

    [Fact]
    public void Items_Clear_Should_Update_UI_WhenObservable()
    {
        // setup
        var items = new ObservableCollection<Item>() {
            new Item()
            {
                Text = "Item 1"
            },
            new Item()
            {
                Text = "Item 2"
            }
        };

        var cut = RenderComponent<TreeViewComponent>( parameters =>
        {
            parameters.Add( x => x.Items, items );
        } );

        items.Clear();

        cut.Render();

        var nodes = cut.FindAll( ".b-tree-view .b-tree-view-node .b-tree-view-node-title" );
        cut.WaitForAssertion( () =>
        {
            nodes.Refresh();
            nodes.Count.Should().Be( 0 );
        } );
    }

    [Fact]
    public void ExpandedNodes_Should_Expand_When_ListMutates()
    {
        var child = new Item() { Text = "Child 1" };
        var parent = new Item()
        {
            Text = "Parent",
            Children = new List<Item> { child }
        };

        var items = new List<Item> { parent };
        var expandedNodes = new List<Item>();

        var cut = RenderComponent<TreeView<Item>>( parameters =>
        {
            parameters.Add( p => p.Nodes, items );
            parameters.Add( p => p.GetChildNodes, (Func<Item, IEnumerable<Item>>)( node => node.Children ) );
            parameters.Add( p => p.HasChildNodes, (Func<Item, bool>)( node => node.Children?.Any() == true ) );
            parameters.Add( p => p.NodeContent, (RenderFragment<Item>)( context => builder => builder.AddContent( 0, context.Text ) ) );
            parameters.Add( p => p.ExpandedNodes, expandedNodes );
        } );

        var nodes = cut.FindAll( ".b-tree-view .b-tree-view-node .b-tree-view-node-title" );
        nodes.Count.Should().Be( 1 );
        nodes[0].TextContent.Should().Contain( "Parent" );

        expandedNodes.Add( parent );

        cut.Render();

        cut.WaitForAssertion( () =>
        {
            var refreshedNodes = cut.FindAll( ".b-tree-view .b-tree-view-node .b-tree-view-node-title" );
            refreshedNodes.Count.Should().Be( 2 );
            refreshedNodes[1].TextContent.Should().Contain( "Child 1" );
        } );
    }

    [Fact]
    public async Task Reload_Should_Update_UI()
    {
        // setup
        var items = new List<Item>() {
            new Item()
            {
                Text = "Item 1"
            },
            new Item()
            {
                Text = "Item 2"
            }
        };

        var cut = RenderComponent<TreeViewComponent>( parameters =>
        {
            parameters.Add( x => x.Items, items );
        } );

        items.Add( new Item()
        {
            Text = "Item 3"
        } );

        var treeView = cut.FindComponent<TreeView<Item>>();

        await treeView.Instance.Reload();

        var nodes = cut.FindAll( ".b-tree-view .b-tree-view-node .b-tree-view-node-title" );
        cut.WaitForAssertion( () =>
        {
            nodes.Refresh();
            nodes.Count.Should().Be( 3 );
        } );
    }

    [Fact]
    public void AutoExpandAll_Should_Expand_Node_When_ObservableChildren_Are_Added_After_InitialRender()
    {
        var children = new ObservableCollection<Item>();
        var root = new Item()
        {
            Text = "Root",
            Children = children
        };

        var items = new ObservableCollection<Item>() { root };

        var cut = RenderComponent<TreeView<Item>>( parameters =>
        {
            parameters.Add( p => p.Nodes, items );
            parameters.Add( p => p.GetChildNodes, (Func<Item, IEnumerable<Item>>)( node => node.Children ) );
            parameters.Add( p => p.HasChildNodes, (Func<Item, bool>)( node => node.Children?.Any() == true ) );
            parameters.Add( p => p.NodeContent, (RenderFragment<Item>)( context => builder => builder.AddContent( 0, context.Text ) ) );
            parameters.Add( p => p.AutoExpandAll, true );
        } );

        var nodes = cut.FindAll( ".b-tree-view .b-tree-view-node .b-tree-view-node-title" );
        nodes.Count.Should().Be( 1 );
        nodes[0].TextContent.Should().Contain( "Root" );

        children.Add( new Item() { Text = "Child 1" } );

        cut.Render();

        cut.WaitForAssertion( () =>
        {
            var refreshedNodes = cut.FindAll( ".b-tree-view .b-tree-view-node .b-tree-view-node-title" );
            refreshedNodes.Count.Should().Be( 2 );
            refreshedNodes[1].TextContent.Should().Contain( "Child 1" );
        } );
    }

    [Fact]
    public async Task ReloadNode_Should_Expand_Node_From_ExpandedNodes()
    {
        var parent = new Item()
        {
            Text = "Parent",
            Children = Array.Empty<Item>(),
        };

        var expandedNodes = new List<Item>();

        var cut = RenderComponent<TreeView<Item>>( parameters =>
        {
            parameters.Add( p => p.Nodes, new[] { parent } );
            parameters.Add( p => p.GetChildNodes, (Func<Item, IEnumerable<Item>>)( node => node.Children ) );
            parameters.Add( p => p.HasChildNodes, (Func<Item, bool>)( node => node.Children?.Any() == true ) );
            parameters.Add( p => p.NodeContent, (RenderFragment<Item>)( context => builder => builder.AddContent( 0, context.Text ) ) );
            parameters.Add( p => p.ExpandedNodes, expandedNodes );
        } );

        parent.Children = new[]
        {
            new Item() { Text = "Child 1" },
        };

        expandedNodes.Add( parent );

        await cut.Instance.ReloadNode( parent );

        cut.WaitForAssertion( () =>
        {
            var nodes = cut.FindAll( ".b-tree-view .b-tree-view-node .b-tree-view-node-title" );
            nodes.Count.Should().Be( 2 );
            nodes[1].TextContent.Should().Contain( "Child 1" );
        } );
    }

    [Fact]
    public async Task DragDrop_Should_Invoke_ItemDropped_With_New_Parent_And_Dragged_Node()
    {
        var dragged = new Item() { Text = "Dragged" };
        var target = new Item() { Text = "Target" };
        TreeViewNodeDragEventArgs<Item> droppedArgs = null;

        var cut = RenderComponent<TreeView<Item>>( parameters =>
        {
            parameters.Add( p => p.Nodes, new[] { dragged, target } );
            parameters.Add( p => p.NodeContent, (RenderFragment<Item>)( context => builder => builder.AddContent( 0, context.Text ) ) );
            parameters.Add( p => p.Draggable, true );
            parameters.Add( p => p.NodeDropped, EventCallback.Factory.Create<TreeViewNodeDragEventArgs<Item>>( this, args => droppedArgs = args ) );
        } );

        var nodeContents = cut.FindAll( ".b-tree-view .b-tree-view-node .b-tree-view-node-title > span" );

        await nodeContents[0].DragStartAsync( new DragEventArgs() );
        nodeContents = cut.FindAll( ".b-tree-view .b-tree-view-node .b-tree-view-node-title > span" );
        await nodeContents[1].DropAsync( new DragEventArgs() { OffsetY = 12 } );

        droppedArgs.Should().NotBeNull();
        droppedArgs.DraggedNode.Should().BeSameAs( dragged );
        droppedArgs.NewParentNode.Should().BeSameAs( target );
        droppedArgs.OldParentNode.Should().BeNull();
        droppedArgs.OldIndex.Should().Be( 0 );
        droppedArgs.NewIndex.Should().Be( 0 );
    }

    [Fact]
    public async Task DragDrop_Should_Include_Old_Parent_And_Indexes()
    {
        var dragged = new Item() { Text = "Dragged" };
        var source = new Item()
        {
            Text = "Source",
            Children = new[] { dragged },
        };
        var target = new Item() { Text = "Target" };
        TreeViewNodeDragEventArgs<Item> droppedArgs = null;
        var expandedNodes = new List<Item> { source };

        var cut = RenderComponent<TreeView<Item>>( parameters =>
        {
            parameters.Add( p => p.Nodes, new[] { source, target } );
            parameters.Add( p => p.GetChildNodes, (Func<Item, IEnumerable<Item>>)( node => node.Children ) );
            parameters.Add( p => p.HasChildNodes, (Func<Item, bool>)( node => node.Children?.Any() == true ) );
            parameters.Add( p => p.ExpandedNodes, expandedNodes );
            parameters.Add( p => p.NodeContent, (RenderFragment<Item>)( context => builder => builder.AddContent( 0, context.Text ) ) );
            parameters.Add( p => p.Draggable, true );
            parameters.Add( p => p.NodeDropped, EventCallback.Factory.Create<TreeViewNodeDragEventArgs<Item>>( this, args => droppedArgs = args ) );
        } );

        var nodeContents = cut.FindAll( ".b-tree-view .b-tree-view-node .b-tree-view-node-title > span" );

        await nodeContents[1].DragStartAsync( new DragEventArgs() );
        await nodeContents[2].DropAsync( new DragEventArgs() { OffsetY = 12 } );

        droppedArgs.Should().NotBeNull();
        droppedArgs.OldParentNode.Should().BeSameAs( source );
        droppedArgs.NewParentNode.Should().BeSameAs( target );
        droppedArgs.OldIndex.Should().Be( 0 );
        droppedArgs.NewIndex.Should().Be( 0 );
    }

    [Fact]
    public async Task DragDrop_Should_Reorder_SubNodes_Within_Same_Parent()
    {
        var child1 = new Item() { Text = "Child 1" };
        var child2 = new Item() { Text = "Child 2" };
        var child3 = new Item() { Text = "Child 3" };
        var parent = new Item()
        {
            Text = "Parent",
            Children = new[] { child1, child2, child3 },
        };
        TreeViewNodeDragEventArgs<Item> droppedArgs = null;
        var expandedNodes = new List<Item> { parent };

        var cut = RenderComponent<TreeView<Item>>( parameters =>
        {
            parameters.Add( p => p.Nodes, new[] { parent } );
            parameters.Add( p => p.GetChildNodes, (Func<Item, IEnumerable<Item>>)( node => node.Children ) );
            parameters.Add( p => p.HasChildNodes, (Func<Item, bool>)( node => node.Children?.Any() == true ) );
            parameters.Add( p => p.ExpandedNodes, expandedNodes );
            parameters.Add( p => p.NodeContent, (RenderFragment<Item>)( context => builder => builder.AddContent( 0, context.Text ) ) );
            parameters.Add( p => p.Draggable, true );
            parameters.Add( p => p.NodeDropped, EventCallback.Factory.Create<TreeViewNodeDragEventArgs<Item>>( this, args => droppedArgs = args ) );
        } );

        var nodeContents = cut.FindAll( ".b-tree-view .b-tree-view-node .b-tree-view-node-title > span" );

        await nodeContents[3].DragStartAsync( new DragEventArgs() );
        nodeContents = cut.FindAll( ".b-tree-view .b-tree-view-node .b-tree-view-node-title > span" );
        await nodeContents[1].DropAsync( new DragEventArgs() { OffsetY = 1 } );

        droppedArgs.Should().NotBeNull();
        droppedArgs.DraggedNode.Should().BeSameAs( child3 );
        droppedArgs.OldParentNode.Should().BeSameAs( parent );
        droppedArgs.NewParentNode.Should().BeSameAs( parent );
        droppedArgs.OldIndex.Should().Be( 2 );
        droppedArgs.NewIndex.Should().Be( 0 );
    }

    [Fact]
    public async Task DragDrop_Should_Support_Drag_And_Drop_Between_SubNodes()
    {
        var dragged = new Item() { Text = "Dragged" };
        var sourceParent = new Item()
        {
            Text = "Source",
            Children = new[] { dragged },
        };
        var targetChild = new Item() { Text = "Target Child" };
        var targetParent = new Item()
        {
            Text = "Target Parent",
            Children = new[] { targetChild },
        };
        TreeViewNodeDragEventArgs<Item> droppedArgs = null;
        var expandedNodes = new List<Item> { sourceParent, targetParent };

        var cut = RenderComponent<TreeView<Item>>( parameters =>
        {
            parameters.Add( p => p.Nodes, new[] { sourceParent, targetParent } );
            parameters.Add( p => p.GetChildNodes, (Func<Item, IEnumerable<Item>>)( node => node.Children ) );
            parameters.Add( p => p.HasChildNodes, (Func<Item, bool>)( node => node.Children?.Any() == true ) );
            parameters.Add( p => p.ExpandedNodes, expandedNodes );
            parameters.Add( p => p.NodeContent, (RenderFragment<Item>)( context => builder => builder.AddContent( 0, context.Text ) ) );
            parameters.Add( p => p.Draggable, true );
            parameters.Add( p => p.NodeDropped, EventCallback.Factory.Create<TreeViewNodeDragEventArgs<Item>>( this, args => droppedArgs = args ) );
        } );

        var nodeContents = cut.FindAll( ".b-tree-view .b-tree-view-node .b-tree-view-node-title > span" );

        await nodeContents[1].DragStartAsync( new DragEventArgs() );
        nodeContents = cut.FindAll( ".b-tree-view .b-tree-view-node .b-tree-view-node-title > span" );
        await nodeContents[3].DropAsync( new DragEventArgs() { OffsetY = 12 } );

        droppedArgs.Should().NotBeNull();
        droppedArgs.DraggedNode.Should().BeSameAs( dragged );
        droppedArgs.OldParentNode.Should().BeSameAs( sourceParent );
        droppedArgs.NewParentNode.Should().BeSameAs( targetChild );
        droppedArgs.OldIndex.Should().Be( 0 );
        droppedArgs.NewIndex.Should().Be( 0 );
    }

    [Fact]
    public async Task DragDrop_Should_Show_Visual_Indicator_On_Active_Drop_Target()
    {
        var dragged = new Item() { Text = "Dragged" };
        var target = new Item() { Text = "Target" };

        var cut = RenderComponent<TreeView<Item>>( parameters =>
        {
            parameters.Add( p => p.Nodes, new[] { dragged, target } );
            parameters.Add( p => p.NodeContent, (RenderFragment<Item>)( context => builder => builder.AddContent( 0, context.Text ) ) );
            parameters.Add( p => p.Draggable, true );
        } );

        var nodeContents = cut.FindAll( ".b-tree-view .b-tree-view-node .b-tree-view-node-title > span" );

        await nodeContents[0].DragStartAsync( new DragEventArgs() );
        nodeContents = cut.FindAll( ".b-tree-view .b-tree-view-node .b-tree-view-node-title > span" );
        await nodeContents[1].DragOverAsync( new DragEventArgs() { OffsetY = 1 } );

        cut.WaitForAssertion( () =>
        {
            var refreshedNodeContents = cut.FindAll( ".b-tree-view .b-tree-view-node .b-tree-view-node-title > span" );
            refreshedNodeContents.Count.Should().Be( 2 );

            refreshedNodeContents[0].ParentElement.ParentElement.ClassList.Should().NotContain( "b-tree-view-node-drop-target" );
            refreshedNodeContents[1].ParentElement.ParentElement.ClassList.Should().Contain( "b-tree-view-node-drop-target" );
        } );
    }
}
