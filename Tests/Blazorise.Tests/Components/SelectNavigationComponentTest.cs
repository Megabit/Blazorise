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
        Services.AddBlazoriseTests().AddBootstrapProvidersTests().AddTestData();
        JSInterop.AddBlazoriseButton();
        Services.AddSingleton<NavigationManager>( new Mock<NavigationManager>().Object );
    }

    [Fact]
    public void TestNavigation()
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
        select.Change( "one" );
        Assert.Equal( "one", result.InnerHtml );

        btnOne.Click();
        Assert.Equal( "one", result.InnerHtml );

        // test 2
        select.Change( "two" );
        Assert.Equal( "two", result.InnerHtml );

        btnTwo.Click();
        Assert.Equal( "two", result.InnerHtml );

        // test 3
        btnOne.Click();
        Assert.Equal( "one", result.InnerHtml );

        btnTwo.Click();
        Assert.Equal( "two", result.InnerHtml );
    }
}