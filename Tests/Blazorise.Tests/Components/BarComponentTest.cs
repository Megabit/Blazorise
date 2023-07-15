#region Using directives
using System.Threading.Tasks;
using BasicTestApp.Client;
using Blazorise.Tests.Helpers;
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;
public class BarComponentTest : TestContext
{
    public BarComponentTest()
    {
        BlazoriseConfig.AddBootstrapProviders( Services );
        BlazoriseConfig.JSInterop.AddBreakpoint( this.JSInterop );
        BlazoriseConfig.JSInterop.AddClosable( this.JSInterop );
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