using System.Threading.Tasks;
using Bunit;
using Xunit;

namespace Blazorise.Tests.Components;

public class ComponentRenderingTest : TestContext
{
    public ComponentRenderingTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop
            .AddBlazoriseTextInput()
            .AddBlazoriseButton();
    }

    [Fact]
    public void CanRenderTextOnlyComponent()
    {
        // setup

        // test
        var appElement = RenderComponent<TextOnlyComponent>();

        // validate
        Assert.Contains( "Hello from TextOnlyComponent", appElement.Markup );
    }

    [Fact]
    public void CanRenderButtonComponent()
    {
        // setup
        var buttonOpen = "<button";
        var buttonClose = "</button>";
        var buttonType = @"type=""button""";
        var buttonContent = "hello primary";

        // test
        var comp = RenderComponent<ButtonOnlyComponent>();

        // validate
        this.JSInterop.VerifyNotInvoke( "initialize" );
        Assert.Contains( buttonOpen, comp.Markup );
        Assert.Contains( buttonClose, comp.Markup );
        Assert.Contains( buttonType, comp.Markup );
        Assert.Contains( buttonContent, comp.Markup );
    }

    [Fact]
    public async Task CannotChangeElementId()
    {
        // setup
        var comp = RenderComponent<ElementIdComponent>();
        var date = comp.Find( "input" );
        var button = comp.Find( "button" );

        Assert.NotEqual( string.Empty, date.GetAttribute( "id" ) );

        // test
        var before = date.GetAttribute( "id" );
        await button.ClickAsync();

        // validate
        this.JSInterop.VerifyNotInvoke( "initialize" );
        Assert.Equal( before, date.GetAttribute( "id" ) );
    }
}