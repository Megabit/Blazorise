#region Using directives
using System.Threading.Tasks;
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class ListGroupComponentTest : TestContext
{
    public ListGroupComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
    }

    [Fact]
    public void SelectableListGroup_ShouldRenderListboxRole()
    {
        // setup
        var comp = RenderComponent<ListGroupComponent>();

        // test
        var listGroupElement = comp.Find( "#selectable-list-group" );

        // validate
        Assert.Equal( "listbox", listGroupElement.GetAttribute( "role" ) );
        Assert.Null( listGroupElement.GetAttribute( "aria-multiselectable" ) );
    }

    [Fact]
    public void SelectableMultiListGroup_ShouldRenderAriaMultiselectable()
    {
        // setup
        var comp = RenderComponent<ListGroupComponent>(
            parameters => parameters.Add( x => x.SelectionMode, ListGroupSelectionMode.Multiple ) );

        // test
        var listGroupElement = comp.Find( "#selectable-list-group" );

        // validate
        Assert.Equal( "listbox", listGroupElement.GetAttribute( "role" ) );
        Assert.Equal( "true", listGroupElement.GetAttribute( "aria-multiselectable" ) );
    }

    [Fact]
    public void SelectableListGroupItems_ShouldBeTabbable()
    {
        // setup
        var comp = RenderComponent<ListGroupComponent>();

        // test
        var firstItem = comp.Find( "#list-group-item-1" );
        var secondItem = comp.Find( "#list-group-item-2" );

        // validate
        Assert.Equal( "option", firstItem.GetAttribute( "role" ) );
        Assert.Equal( "option", secondItem.GetAttribute( "role" ) );
        Assert.Equal( "false", firstItem.GetAttribute( "aria-selected" ) );
        Assert.Equal( "false", secondItem.GetAttribute( "aria-selected" ) );
        Assert.Equal( "0", firstItem.GetAttribute( "tabindex" ) );
        Assert.Equal( "0", secondItem.GetAttribute( "tabindex" ) );
    }

    [Fact]
    public async Task Enter_OnFocusedSelectableItem_ShouldSelectItem()
    {
        // setup
        var comp = RenderComponent<ListGroupComponent>();
        var secondItem = comp.Find( "#list-group-item-2" );

        // test
        await secondItem.KeyDownAsync( new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs() { Key = "Enter" } );

        // validate
        var selectedItemElement = comp.Find( "#list-group-selected-item" );
        Assert.Equal( "item-2", selectedItemElement.TextContent );
        Assert.Equal( "false", comp.Find( "#list-group-item-1" ).GetAttribute( "aria-selected" ) );
        Assert.Equal( "true", comp.Find( "#list-group-item-2" ).GetAttribute( "aria-selected" ) );
    }

    [Fact]
    public async Task Space_OnFocusedSelectableItem_ShouldSelectItem()
    {
        // setup
        var comp = RenderComponent<ListGroupComponent>();
        var firstItem = comp.Find( "#list-group-item-1" );

        // test
        await firstItem.KeyDownAsync( new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs() { Key = " " } );

        // validate
        var selectedItemElement = comp.Find( "#list-group-selected-item" );
        Assert.Equal( "item-1", selectedItemElement.TextContent );
        Assert.Equal( "true", comp.Find( "#list-group-item-1" ).GetAttribute( "aria-selected" ) );
        Assert.Equal( "false", comp.Find( "#list-group-item-2" ).GetAttribute( "aria-selected" ) );
    }
}