using BasicTestApp.Client;
using Blazorise.Tests.Helpers;
using Bunit;
using Xunit;

namespace Blazorise.Tests.Components
{
    public class TextEditComponentTest : TestContext
    {
        public TextEditComponentTest()
        {
            BlazoriseConfig.AddBootstrapProviders( Services );
        }

        [Fact]
        public void CanChangeText()
        {
            // setup
            var comp = RenderComponent<TextEditComponent>();
            var paragraph = comp.Find( "#text-basic" );
            var text = comp.Find( "input" );

            Assert.Null( text.GetAttribute( "value" ) );

            // test
            text.Input( "abc" );

            // validate
            Assert.Contains( "abc", text.GetAttribute( "value" ) );
        }

        [Fact]
        public void CanChangeTextUsingEvent()
        {
            // setup
            var comp = RenderComponent<TextEditComponent>();
            var paragraph = comp.Find( "#text-event-initially-blank" );
            var text = comp.Find( "#text-with-event" );
            var result = comp.Find( "#text-event-initially-blank-result" );

            Assert.Equal( string.Empty, result.InnerHtml );

            // test initial
            text.Input( "abcde" );
            Assert.Equal( "abcde", result.InnerHtml );

            // test additional text
            text.Input( "abcdefghijklmnopqrstuvwxyz" );
            Assert.Equal( "abcdefghijklmnopqrstuvwxyz", result.InnerHtml );

            // text backspace.
            // todo: figure out how to set special keys.
            // text.KeyPress( "Keys.Backspace" );
            // Assert.Equal( "abcdefghijklmnopqrstuvwxy", result.InnerHtml );
        }

        [Fact]
        public void CanChangeTextUsingBind()
        {
            // setup
            var comp = RenderComponent<TextEditComponent>();
            var paragraph = comp.Find( "#text-bind-initially-blank" );
            var text = comp.Find( "#text-binding" );
            var result = comp.Find( "#text-bind-initially-blank-result" );

            Assert.Equal( string.Empty, result.InnerHtml );

            // test additional text
            text.Input( "abcdefghijklmnopqrstuvwxyz" );
            Assert.Equal( "abcdefghijklmnopqrstuvwxyz", result.InnerHtml );

            // text backspace.
            // todo: figure out how to set special keys.
            // text.KeyPress( "Keys.Backspace" );
            // Assert.Equal( "abcdefghijklmnopqrstuvwxy", result.InnerHtml );
        }
    }
}