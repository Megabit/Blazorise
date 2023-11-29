using Blazorise.Tests.TestServices;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Blazorise.Tests.Components;

public class BarLinkComponentTest : TestContext
{
    public BarLinkComponentTest()
    {
        var testServices = new TestServiceProvider( Services.AddSingleton<NavigationManager, TestNavigationManager>() );

        testServices.AddBlazoriseTests().AddBootstrapProvidersTests();
        JSInterop
            .AddBlazoriseButton();
    }

    [Fact]
    public void CanRaiseClicked_WithoutToParameterSet()
    {
        // setup
        bool wasClicked = false;
        var testCallback = new EventCallback<MouseEventArgs>( null, ( MouseEventArgs e ) => wasClicked = true );

        // test
        var comp = RenderComponent<BarLink>( builder =>
            builder
                .Add( p => p.Clicked, testCallback ) );

        comp.Find( "a" ).Click();

        // validate
        Assert.True( wasClicked );
    }

    [Fact]
    public void CanRaiseClicked_WithToParameterSet()
    {
        // setup
        bool wasClicked = false;
        var testCallback = new EventCallback<MouseEventArgs>( null, ( MouseEventArgs e ) => wasClicked = true );

        // test
        var comp = RenderComponent<BarLink>( builder =>
            builder
                .Add( p => p.To, "test" )
                .Add( p => p.Clicked, testCallback ) );

        var link = comp.FindComponent<Link>();
        link.Find( "a" ).Click();

        // validate
        Assert.True( wasClicked );
    }
}