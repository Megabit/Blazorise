using System.ComponentModel;
using System.Threading.Tasks;
using Bunit;
using Xunit;

namespace Blazorise.Tests.Components;

public class SelectErrorBoundaryComponentTest : TestContext
{
    public SelectErrorBoundaryComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseButton();
    }

    [Fact]
    public async Task ValueChanged_Exception_TriggersErrorBoundary()
    {
        var comp = RenderComponent<SelectErrorBoundaryComponent>();
        var select = comp.Find( "select" );

        await select.ChangeAsync( "2" );

        comp.WaitForAssertion( () =>
        {
            var error = comp.Find( "#select-error-boundary-error" );
            Assert.Equal( nameof( InvalidEnumArgumentException ), error.TextContent );
        } );
    }
}