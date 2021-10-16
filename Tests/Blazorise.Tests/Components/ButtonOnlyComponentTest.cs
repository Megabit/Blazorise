#region Using directives
using BasicTestApp.Client;
using Blazorise.Tests.Helpers;
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components
{
    public class ButtonOnlyComponentTest : TestContext
    {
        public ButtonOnlyComponentTest()
        {
            BlazoriseConfig.AddBootstrapProviders( Services );
            BlazoriseConfig.JSInterop.AddButton( this.JSInterop );
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
}
