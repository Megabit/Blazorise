#region Using directives
using System.Threading.Tasks;
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class CloseButtonComponentTest : BunitContext
{
    public CloseButtonComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop
            .AddBlazoriseUtilities()
            .AddBlazoriseClosable();
    }

    [Fact]
    public void RenderTest()
    {
        // setup
        var buttonOpen = "<button";
        var buttonClose = "</button>";
        var buttonContent = "Count";
        var counterOutput = @"<span id=""close-button-event-result"">0</span>";

        // test
        var comp = Render<CloseButtonComponent>();

        // validate
        Assert.Contains( buttonOpen, comp.Markup );
        Assert.Contains( buttonClose, comp.Markup );
        Assert.Contains( buttonContent, comp.Markup );
        Assert.Contains( counterOutput, comp.Markup );
        Assert.NotNull( comp.Find( "#close-button-event" ) );
        Assert.NotNull( comp.Find( "#close-button" ) );
        Assert.NotNull( comp.Find( "#close-button-event-result" ) );
    }

    [Fact]
    public async Task CanRaiseCallback()
    {
        // setup
        var comp = Render<CloseButtonComponent>();
        var result = comp.Find( "#close-button-event-result" );
        var button = comp.Find( "#close-button" );

        // test
        await button.ClickAsync( new() { Button = 0 } );
        var result1 = result.InnerHtml;

        await button.ClickAsync( new() { Button = 0 } );
        var result2 = result.InnerHtml;

        // validate
        Assert.Equal( "1", result1 );
        Assert.Equal( "2", result2 );
    }

    [Fact]
    public async Task CanAutoClose()
    {
        // setup
        var comp = Render<CloseButtonComponent>();
        var result = comp.Find( "#autoclose-button-event-result" );
        var button = comp.Find( "#autoclose-button" );

        // test
        await button.ClickAsync( new() { Button = 0 } );
        var result1 = result.InnerHtml;

        // validate
        Assert.Equal( "1", result1 );
    }

    [Fact]
    public async Task CanAutoCloseOffcanvas()
    {
        // setup
        var comp = Render<CloseButtonOffcanvasComponent>();
        var result = comp.Find( "#autoclose-offcanvas-button-event-result" );
        var button = comp.Find( "#autoclose-offcanvas-button" );

        // test
        await button.ClickAsync( new() { Button = 0 } );
        var result1 = result.InnerHtml;

        // validate
        Assert.Equal( "1", result1 );
    }
}