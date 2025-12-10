#region Using directives
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.TreeView;
using Bunit;
using Microsoft.AspNetCore.Components;
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
}