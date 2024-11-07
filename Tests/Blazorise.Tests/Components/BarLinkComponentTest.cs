using System.Threading.Tasks;
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

        testServices.AddBlazoriseTests().AddBootstrapProviders();
        JSInterop
            .AddBlazoriseButton();
    }

    [Fact]
    public async Task CanRaiseClicked_WithoutToParameterSet()
    {
        // setup
        bool wasClicked = false;
        var testCallback = new EventCallback<MouseEventArgs>( null, ( MouseEventArgs e ) => wasClicked = true );

        // test
        var comp = RenderComponent<BarLink>( builder =>
            builder
                .Add( p => p.Clicked, testCallback ) );

        await comp.Find( "a" ).ClickAsync();

        // validate
        Assert.True( wasClicked );
    }

    [Fact]
    public async Task CanRaiseClicked_WithToParameterSet()
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
        await link.Find( "a" ).ClickAsync();

        // validate
        Assert.True( wasClicked );
    }
}