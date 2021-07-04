#region Using directives

using System.Threading.Tasks;
using BasicTestApp.Client;
using Blazorise.Tests.Helpers;
using Bunit;
using VerifyXunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components
{
    [UsesVerify]
    public class CloseButtonComponentTest : TestContext
    {
        public CloseButtonComponentTest()
        {
            BlazoriseConfig.AddBootstrapProviders( Services );
        }

        [Fact]
        public Task RenderTest()
        {
            // test
            var comp = RenderComponent<CloseButtonComponent>();

            // validate
            return Verifier.Verify( comp );
        }

        [Fact]
        public void CanRaiseCallback()
        {
            // setup
            var comp = RenderComponent<CloseButtonComponent>();
            var result = comp.Find( "#close-button-event-result" );
            var button = comp.Find( "#close-button" );

            // test
            button.Click();
            var result1 = result.InnerHtml;

            button.Click();
            var result2 = result.InnerHtml;

            // validate
            Assert.Equal( "1", result1 );
            Assert.Equal( "2", result2 );
        }

        [Fact]
        public void CanAutoClose()
        {
            // setup
            var comp = RenderComponent<CloseButtonComponent>();
            var result = comp.Find( "#autoclose-button-event-result" );
            var button = comp.Find( "#autoclose-button" );

            // test
            button.Click();
            var result1 = result.InnerHtml;

            // validate
            Assert.Equal( "1", result1 );
        }
    }
}
