using System.Threading.Tasks;
using Bunit;
using Xunit;

namespace Blazorise.Tests.Components;

public class TextEditComponentTest : TestContext
{
    public TextEditComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseTextEdit();
    }

    [Fact]
    public async Task CanChangeText()
    {
        // setup
        var comp = RenderComponent<TextEditComponent>();
        var paragraph = comp.Find( "#text-basic" );
        var text = comp.Find( "input" );

        Assert.Null( text.GetAttribute( "value" ) );

        // test
        await text.InputAsync( new Microsoft.AspNetCore.Components.ChangeEventArgs() { Value = "abc" } );

        // validate
        Assert.Contains( "abc", text.GetAttribute( "value" ) );
    }

    [Fact]
    public async Task CanChangeTextUsingEvent()
    {
        // setup
        var comp = RenderComponent<TextEditComponent>();
        var paragraph = comp.Find( "#text-event-initially-blank" );
        var text = comp.Find( "#text-with-event" );
        var result = comp.Find( "#text-event-initially-blank-result" );

        Assert.Equal( string.Empty, result.InnerHtml );

        // test initial
        await text.InputAsync( new Microsoft.AspNetCore.Components.ChangeEventArgs() { Value = "abcde" } );
        Assert.Equal( "abcde", result.InnerHtml );

        // test additional text
        await text.InputAsync( new Microsoft.AspNetCore.Components.ChangeEventArgs() { Value = "abcdefghijklmnopqrstuvwxyz" } );
        Assert.Equal( "abcdefghijklmnopqrstuvwxyz", result.InnerHtml );

        // text backspace.
        // todo: figure out how to set special keys.
        // text.KeyPress( "Keys.Backspace" );
        // Assert.Equal( "abcdefghijklmnopqrstuvwxy", result.InnerHtml );
    }

    [Fact]
    public async Task CanChangeTextUsingBind()
    {
        // setup
        var comp = RenderComponent<TextEditComponent>();
        var paragraph = comp.Find( "#text-bind-initially-blank" );
        var text = comp.Find( "#text-binding" );
        var result = comp.Find( "#text-bind-initially-blank-result" );

        Assert.Equal( string.Empty, result.InnerHtml );

        // test additional text
        await text.InputAsync( new Microsoft.AspNetCore.Components.ChangeEventArgs() { Value = "abcdefghijklmnopqrstuvwxyz" } );
        Assert.Equal( "abcdefghijklmnopqrstuvwxyz", result.InnerHtml );

        // text backspace.
        // todo: figure out how to set special keys.
        // text.KeyPress( "Keys.Backspace" );
        // Assert.Equal( "abcdefghijklmnopqrstuvwxy", result.InnerHtml );
    }
}