using System.Threading.Tasks;
using Bunit;
using Xunit;

namespace Blazorise.Tests.Components;

public class DropdownComponentTest : TestContext
{
    public DropdownComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProvidersTests().AddTestData();
        JSInterop
            .AddBlazoriseClosable()
            .AddBlazoriseDropdown();
    }

    [Fact]
    public void CanShowAndHideDropdownComponent()
    {
        // setup
        var comp = RenderComponent<DropdownComponent>();
        var drpElement = comp.Find( "#dropdown" );
        var btnElement = comp.Find( "button" );
        var mnuElement = comp.Find( "#dropdown-menu" );

        // test
        btnElement.Click();

        // validate
        Assert.Contains( "show", drpElement.GetAttribute( "class" ) );
        Assert.Contains( "show", mnuElement.GetAttribute( "class" ) );

        // test
        btnElement.Click();

        // validate
        this.JSInterop.VerifyInvoke( "registerClosableComponent" );
        Assert.DoesNotContain( "show", drpElement.GetAttribute( "class" ) );
        Assert.DoesNotContain( "show", mnuElement.GetAttribute( "class" ) );
    }


    [Fact]
    public async Task Checkbox_Should_RenderCheckbox()
    {
        // setup
        var comp = RenderComponent<DropdownComponent>(
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
            checkComp.Instance.Checked.Should().BeFalse();
        } );
    }

    [Fact]
    public async Task Checkbox_Should_Be_Editable()
    {
        // setup
        var comp = RenderComponent<DropdownComponent>(
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
            checkComp.Instance.Checked.Should().BeTrue();
        } );
    }

}