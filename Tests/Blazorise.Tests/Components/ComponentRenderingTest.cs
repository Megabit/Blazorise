using BasicTestApp.Client;
using Blazorise.Tests.Helpers;
using Bunit;
using Xunit;

namespace Blazorise.Tests.Components
{
    public class ComponentRenderingTest : TestContext
    {
        public ComponentRenderingTest()
        {
            BlazoriseConfig.AddBootstrapProviders( Services );
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
            Assert.Contains( buttonOpen, comp.Markup );
            Assert.Contains( buttonClose, comp.Markup );
            Assert.Contains( buttonType, comp.Markup );
            Assert.Contains( buttonContent, comp.Markup );
        }

        [Fact]
        public void CannotChangeElementId()
        {
            // setup
            var comp = RenderComponent<ElementIdComponent>();
            var date = comp.Find( "input" );
            var button = comp.Find( "button" );

            Assert.NotEqual( string.Empty, date.GetAttribute( "id" ) );

            // test
            var before = date.GetAttribute( "id" );
            button.Click();

            // validate
            Assert.Equal( before, date.GetAttribute( "id" ) );
        }
    }
}
