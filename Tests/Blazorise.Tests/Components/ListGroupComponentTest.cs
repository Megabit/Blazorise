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
    public void SelectableListGroupItems_ShouldBeTabbable()
    {
        // setup
        var comp = RenderComponent<ListGroupComponent>();

        // test
        var firstItem = comp.Find( "#list-group-item-1" );
        var secondItem = comp.Find( "#list-group-item-2" );

        // validate
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
    }
}