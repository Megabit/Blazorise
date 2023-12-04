#region Using directives
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class ButtonOnlyComponentTest : TestContext
{
    public ButtonOnlyComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop
            .AddBlazoriseButton();
    }

    [Fact]
    public void RenderTest()
    {
        // setup
        var buttonOpen = "<button";
        var buttonClose = "</button>";
        var buttonType = @"type=""button""";
        var buttonContent = "hello primary";

        // test
        var comp = RenderComponent<ButtonOnlyComponent>();

        // validate
        this.JSInterop.VerifyInvoke( "initialize" );
        Assert.Contains( buttonOpen, comp.Markup );
        Assert.Contains( buttonClose, comp.Markup );
        Assert.Contains( buttonType, comp.Markup );
        Assert.Contains( buttonContent, comp.Markup );
    }
}