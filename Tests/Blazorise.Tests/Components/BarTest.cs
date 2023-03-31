#region Using directives
#endregion

using System.Threading.Tasks;
using BasicTestApp.Client;
using Blazorise.Tests.Helpers;
using Bunit;
using Xunit;

namespace Blazorise.Tests.Components;
public class BarComponentTest : TestContext
{
    public BarComponentTest()
    {
        BlazoriseConfig.AddBootstrapProviders( Services );
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
        var barItem = comp.Find( ".dropdown-toggle .nav-item" );
        Assert.Equal( "1", barItem.TextContent );
    }
}