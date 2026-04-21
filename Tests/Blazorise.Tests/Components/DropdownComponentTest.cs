using System.Threading.Tasks;
using Bunit;
using Xunit;

namespace Blazorise.Tests.Components;

public class DropdownComponentTest : BunitContext
{
    public DropdownComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop
            .AddBlazoriseClosable()
            .AddBlazoriseDropdown();
    }

    [Fact]
    public async Task CanShowAndHideDropdownComponent()
    {
        // setup
        var comp = Render<DropdownComponent>();
        var drpElement = comp.Find( "#dropdown" );
        var btnElement = comp.Find( "button" );
        var mnuElement = comp.Find( "#dropdown-menu" );

        // test
        await btnElement.ClickAsync();

        // validate
        Assert.Contains( "show", drpElement.GetAttribute( "class" ) );
        Assert.Contains( "show", mnuElement.GetAttribute( "class" ) );

        // test
        await btnElement.ClickAsync();

        // validate
        this.JSInterop.VerifyInvoke( "registerClosableComponent" );
        Assert.DoesNotContain( "show", drpElement.GetAttribute( "class" ) );
        Assert.DoesNotContain( "show", mnuElement.GetAttribute( "class" ) );
    }

    [Fact]
    public async Task Enter_OnFocusedItem_Should_SelectItem_AndCloseDropdown()
    {
        // setup
        var comp = Render<DropdownComponent>();
        var drpElement = comp.Find( "#dropdown" );
        var btnElement = comp.Find( "button" );
        var mnuElement = comp.Find( "#dropdown-menu" );

        // test
        await btnElement.ClickAsync();

        var firstMenuItemElement = comp.Find( "#dropdown-menu a" );
        await firstMenuItemElement.KeyDownAsync( new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs() { Key = "Enter" } );

        // validate
        Assert.DoesNotContain( "show", drpElement.GetAttribute( "class" ) );
        Assert.DoesNotContain( "show", mnuElement.GetAttribute( "class" ) );
    }

    [Fact]
    public void DropdownMenu_ShouldHaveRoleMenu()
    {
        // setup
        var comp = Render<DropdownComponent>();

        // test
        var menuElement = comp.Find( "#dropdown-menu" );

        // validate
        Assert.Equal( "menu", menuElement.GetAttribute( "role" ) );
    }

    [Fact]
    public void DropdownItem_ShouldHaveRoleMenuitem()
    {
        // setup
        var comp = Render<DropdownComponent>();

        // test
        var firstMenuItemElement = comp.Find( "#dropdown-menu a" );

        // validate
        Assert.Equal( "menuitem", firstMenuItemElement.GetAttribute( "role" ) );
    }

    [Fact]
    public void DropdownItem_Should_Have_AriaLabelledBy_That_PointsToItemText()
    {
        // setup
        var comp = Render<DropdownComponent>();

        // test
        var firstMenuItemElement = comp.Find( "#dropdown-menu a" );
        var ariaLabelledBy = firstMenuItemElement.GetAttribute( "aria-labelledby" );

        // validate
        Assert.False( string.IsNullOrEmpty( ariaLabelledBy ) );

        var labelElement = comp.Find( $"#{ariaLabelledBy}" );
        Assert.Contains( "Item 1", labelElement.TextContent );
    }


    [Fact]
    public async Task Checkbox_Should_RenderCheckbox()
    {
        // setup
        var comp = Render<DropdownComponent>(
            p => p.Add( x => x.ShowCheckbox, true ) );
        var drpElement = comp.Find( "#dropdown" );
        var btnElement = comp.Find( "button" );
        var mnuElement = comp.Find( "#dropdown-menu" );

        // test
        await btnElement.ClickAsync();

        // validate
        comp.WaitForAssertion( () =>
        {
            var checkComp = comp.FindComponent<Check<bool>>();
            checkComp.Instance.Value.Should().BeFalse();
        } );
    }

    [Fact]
    public async Task Checkbox_Should_Be_Editable()
    {
        // setup
        var comp = Render<DropdownComponent>(
            p => p.Add( x => x.ShowCheckbox, true ) );
        var drpElement = comp.Find( "#dropdown" );
        var btnElement = comp.Find( "button" );
        var mnuElement = comp.Find( "#dropdown-menu" );

        // test
        await btnElement.ClickAsync();

        var checkbox = comp.Find( "input[type=checkbox]" );
        await checkbox.ChangeAsync( new Microsoft.AspNetCore.Components.ChangeEventArgs() { Value = true } );

        // validate
        comp.WaitForAssertion( () =>
        {
            var checkComp = comp.FindComponent<Check<bool>>();
            checkComp.Instance.Value.Should().BeTrue();
        } );
    }

}