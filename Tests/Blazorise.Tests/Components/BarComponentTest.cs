#region Using directives
using System.Threading.Tasks;
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class BarComponentTest : BunitContext
{
    public BarComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop
            .AddBlazoriseBreakpoint()
            .AddBlazoriseClosable()
            .AddBlazoriseDropdown();
    }

    [Fact]
    public async Task BarTroggle_ShouldTriggerClicked()
    {
        // setup
        var comp = Render<BarComponent>();
        var barToggler = comp.Find( ".dropdown-toggle" );

        // test
        await barToggler.ClickAsync( new() );

        // validate
        var barItem = comp.Find( ".dropdown-menu .nav-item" );
        Assert.Equal( "1", barItem.TextContent );
    }

    [Fact]
    public async Task BarDropdownToggle_Enter_ShouldCloseDropdownWhenVisible()
    {
        // setup
        var comp = Render<BarComponent>();
        var barToggle = comp.Find( ".dropdown-toggle" );
        var dropdownMenu = comp.Find( ".dropdown-menu" );

        // test
        await barToggle.ClickAsync( new() );
        await barToggle.KeyDownAsync( new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs() { Key = "Enter" } );

        // validate
        Assert.DoesNotContain( "show", dropdownMenu.GetAttribute( "class" ) );
    }

    [Fact]
    public async Task BarDropdownItem_Enter_ShouldTriggerClickedAndCloseDropdown()
    {
        // setup
        var comp = Render<BarComponent>();
        var barToggle = comp.Find( ".dropdown-toggle" );
        var dropdownMenu = comp.Find( ".dropdown-menu" );

        // test
        await barToggle.ClickAsync( new() );

        var dropdownItem = comp.Find( "#bar-dropdown-item" );
        await dropdownItem.KeyDownAsync( new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs() { Key = "Enter" } );

        // validate
        Assert.Contains( "Item 1", dropdownItem.TextContent );
        Assert.DoesNotContain( "show", dropdownMenu.GetAttribute( "class" ) );
    }

    [Fact]
    public void BarDropdownMenu_ShouldHaveRoleMenu()
    {
        // setup
        var comp = Render<BarComponent>();

        // test
        var dropdownMenu = comp.Find( ".dropdown-menu" );

        // validate
        Assert.Equal( "menu", dropdownMenu.GetAttribute( "role" ) );
    }

    [Fact]
    public void BarDropdownItem_ShouldHaveRoleMenuitem()
    {
        // setup
        var comp = Render<BarComponent>();

        // test
        var dropdownItem = comp.Find( "#bar-dropdown-item" );

        // validate
        Assert.Equal( "menuitem", dropdownItem.GetAttribute( "role" ) );
    }

    [Fact]
    public void BarDropdownItem_ShouldHaveAriaLabelledBy_ThatPointsToItemText()
    {
        // setup
        var comp = Render<BarComponent>();

        // test
        var dropdownItem = comp.Find( "#bar-dropdown-item" );
        var ariaLabelledBy = dropdownItem.GetAttribute( "aria-labelledby" );

        // validate
        Assert.False( string.IsNullOrEmpty( ariaLabelledBy ) );

        var labelElement = comp.Find( $"#{ariaLabelledBy}" );
        Assert.Contains( "Item 0", labelElement.TextContent );
    }
}
