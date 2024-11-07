using System.Threading.Tasks;
using Bunit;
using Xunit;

namespace Blazorise.Tests.Components;

public class RadioGroupComponentTest : TestContext
{
    public RadioGroupComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseUtilities();
    }

    [Fact]
    public async Task CanCheckString_InitiallyChecked()
    {
        // setup
        var comp = RenderComponent<RadioGroupComponent>();
        var para = comp.Find( "#radiogroup-event-initially-selected" );
        var radioR = comp.Find( ".radioR" );
        var radioG = comp.Find( ".radioG" );
        var radioB = comp.Find( ".radioB" );
        var result = comp.Find( "#radiogroup-event-initially-selected-result" );

        // test 1
        Assert.NotNull( radioG.GetAttribute( "checked" ) );
        Assert.Equal( "green", result.InnerHtml );
        Assert.Null( radioR.GetAttribute( "checked" ) );
        Assert.Null( radioB.GetAttribute( "checked" ) );

        // test 2
        await radioR.ChangeAsync( "red" );

        Assert.Null( radioG.GetAttribute( "checked" ) );
        Assert.NotNull( radioR.GetAttribute( "checked" ) );
        Assert.Equal( "red", result.InnerHtml );

        // test 3
        await radioB.ChangeAsync( "blue" );

        Assert.Null( radioR.GetAttribute( "checked" ) );
        Assert.NotNull( radioB.GetAttribute( "checked" ) );
        Assert.Equal( "blue", result.InnerHtml );
    }
}