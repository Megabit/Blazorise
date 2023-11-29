#region Using directives
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class ButtonComponentTest : TestContext
{
    public ButtonComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProvidersTests().AddTestData();
        JSInterop
            .AddBlazoriseButton();
    }

    [Fact]
    public void RenderTest()
    {
        // setup
        var buttonOpen = "<button";
        var buttonClose = "</button>";
        var buttonContent = "Count";
        var counterOutput = @"<span id=""basic-button-event-result"">0</span>";

        // test
        var comp = RenderComponent<ButtonComponent>();

        // validate
        this.JSInterop.VerifyInvoke( "initialize" );
        Assert.Contains( buttonOpen, comp.Markup );
        Assert.Contains( buttonClose, comp.Markup );
        Assert.Contains( buttonContent, comp.Markup );
        Assert.Contains( counterOutput, comp.Markup );
        Assert.NotNull( comp.Find( "#basic-button-event" ) );
        Assert.NotNull( comp.Find( "#basic-button" ) );
        Assert.NotNull( comp.Find( "#basic-button-event-result" ) );
    }


    [Fact]
    public void CanRaiseCallback()
    {
        // setup
        var comp = RenderComponent<ButtonComponent>();

        var result = comp.Find( "#basic-button-event-result" );
        var button = comp.Find( "#basic-button" );

        // test
        button.Click();
        var result1 = result.InnerHtml;

        button.Click();
        var result2 = result.InnerHtml;

        // validate
        this.JSInterop.VerifyInvoke( "initialize" );
        Assert.Equal( "1", result1 );
        Assert.Equal( "2", result2 );
    }
}