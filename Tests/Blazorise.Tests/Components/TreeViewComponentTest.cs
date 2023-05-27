#region Using directives
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BasicTestApp.Client;
using Blazorise.Tests.Extensions;
using Blazorise.Tests.Helpers;
using Blazorise.TreeView;
using Bunit;
using FluentAssertions;
using Xunit;
using static BasicTestApp.Client.TreeViewComponent;
#endregion

namespace Blazorise.Tests.Components;

public class TreeViewComponentTest : TestContext
{
    public TreeViewComponentTest()
    {
        BlazoriseConfig.AddBootstrapProviders( Services );
        BlazoriseConfig.JSInterop.AddDataGrid( this.JSInterop );
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