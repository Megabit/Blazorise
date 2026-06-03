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
    public async Task BarDropdownToggle_FocusOut_ShouldCloseDropdownWhenVisible()
    {
        // setup
        var comp = Render<BarComponent>();
        var barToggle = comp.Find( ".dropdown-toggle" );
        var dropdownMenu = comp.Find( ".dropdown-menu" );

        // test
        await barToggle.KeyDownAsync( new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs() { Key = "Enter" } );
        await barToggle.TriggerEventAsync( "onfocusout", new Microsoft.AspNetCore.Components.Web.FocusEventArgs() );

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
    public void BarDropdownToggle_ShouldHaveAriaHasPopupMenu()
    {
        // setup
        var comp = Render<BarComponent>();

        // test
        var barToggle = comp.Find( ".dropdown-toggle" );

        // validate
        Assert.Equal( "menu", barToggle.GetAttribute( "aria-haspopup" ) );
    }

    [Fact]
    public async Task BarDropdownToggle_ShouldUpdateAriaExpanded()
    {
        // setup
        var comp = Render<BarComponent>();
        var barToggle = comp.Find( ".dropdown-toggle" );

        // validate
        Assert.Equal( "false", barToggle.GetAttribute( "aria-expanded" ) );

        // test
        await barToggle.ClickAsync( new() );
        barToggle = comp.Find( ".dropdown-toggle" );

        // validate
        Assert.Equal( "true", barToggle.GetAttribute( "aria-expanded" ) );
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

    [Fact]
    public async Task BarDropdownMenu_MouseOver_ShouldKeepVerticalDropdownVisible()
    {
        // setup
        var comp = Render<BarNestedDropdownComponent>();
        var parentToggle = comp.Find( "#parent-toggle" );

        // test
        await parentToggle.TriggerEventAsync( "onmouseenter", new Microsoft.AspNetCore.Components.Web.MouseEventArgs() );

        var parentMenu = comp.Find( "#parent-menu" );
        var leaveTask = parentMenu.TriggerEventAsync( "onmouseleave", new Microsoft.AspNetCore.Components.Web.MouseEventArgs() );
        await Task.Delay( 10 );
        await parentMenu.TriggerEventAsync( "onmouseover", new Microsoft.AspNetCore.Components.Web.MouseEventArgs() );
        await leaveTask;

        // validate
        parentMenu = comp.Find( "#parent-menu" );
        Assert.Contains( "show", parentMenu.GetAttribute( "class" ) );
    }

    [Fact]
    public async Task BarDropdownSubmenu_MouseOverParent_ShouldKeepParentDropdownVisible()
    {
        // setup
        var comp = Render<BarNestedDropdownComponent>();
        var parentToggle = comp.Find( "#parent-toggle" );
        var childToggle = comp.Find( "#child-toggle" );

        await parentToggle.TriggerEventAsync( "onmouseenter", new Microsoft.AspNetCore.Components.Web.MouseEventArgs() );
        await childToggle.TriggerEventAsync( "onmouseenter", new Microsoft.AspNetCore.Components.Web.MouseEventArgs() );

        var parentMenu = comp.Find( "#parent-menu" );
        var childMenu = comp.Find( "#child-menu" );

        // test
        var leaveTask = childMenu.TriggerEventAsync( "onmouseleave", new Microsoft.AspNetCore.Components.Web.MouseEventArgs() );
        await Task.Delay( 10 );
        await parentMenu.TriggerEventAsync( "onmouseover", new Microsoft.AspNetCore.Components.Web.MouseEventArgs() );
        await leaveTask;

        // validate
        parentMenu = comp.Find( "#parent-menu" );
        childMenu = comp.Find( "#child-menu" );
        Assert.Contains( "show", parentMenu.GetAttribute( "class" ) );
        Assert.DoesNotContain( "show", childMenu.GetAttribute( "class" ) );
    }
}