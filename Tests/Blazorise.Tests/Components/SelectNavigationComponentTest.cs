using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Blazorise.Tests.Components;

public class SelectNavigationComponentTest : TestContext
{
    public SelectNavigationComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseButton();
        Services.AddSingleton<NavigationManager>( new Mock<NavigationManager>().Object );
    }

    [Fact]
    public async Task TestNavigation()
    {
        // setup
        var comp = RenderComponent<SelectNavigationsComponent>();
        var paragraph = comp.Find( "#select-value-initialy-selected" );
        var select = comp.Find( "select" );
        var result = comp.Find( "#select-value-initialy-selected-result" );
        var btnOne = comp.Find( ".btn-primary" );
        var btnTwo = comp.Find( ".btn-secondary" );

        Assert.Equal( "two", result.InnerHtml );

        // test 1
        await select.ChangeAsync( "one" );
        Assert.Equal( "one", result.InnerHtml );

        await btnOne.ClickAsync();
        Assert.Equal( "one", result.InnerHtml );

        // test 2
        await select.ChangeAsync( "two" );
        Assert.Equal( "two", result.InnerHtml );

        await btnTwo.ClickAsync();
        Assert.Equal( "two", result.InnerHtml );

        // test 3
        await btnOne.ClickAsync();
        Assert.Equal( "one", result.InnerHtml );

        await btnTwo.ClickAsync();
        Assert.Equal( "two", result.InnerHtml );
    }
}