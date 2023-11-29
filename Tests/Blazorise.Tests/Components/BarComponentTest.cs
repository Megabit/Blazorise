#region Using directives
using System.Threading.Tasks;
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;
public class BarComponentTest : TestContext
{
    public BarComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProvidersTests().AddTestData();
        JSInterop
            .AddBlazoriseBreakpoint()
            .AddBlazoriseClosable();
    }

    [Fact]
    public async Task BarTroggle_ShouldTriggerClicked()
    {
        // setup
        var comp = RenderComponent<BarComponent>();
        var barToggler = comp.Find( ".dropdown-toggle" );

        // test
        await barToggler.ClickAsync( new() );

        // validate
        var barItem = comp.Find( ".dropdown-menu .nav-item" );
        Assert.Equal( "1", barItem.TextContent );
    }
}