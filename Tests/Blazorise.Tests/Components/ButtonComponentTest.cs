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
    public class ButtonComponentTest : TestContext
    {
        public ButtonComponentTest()
        {
            BlazoriseConfig.AddBootstrapProviders( Services );
        }

        [Fact]
        public Task RenderTest()
        {
            // test
            var comp = RenderComponent<ButtonComponent>();

            // validate
            return Verifier.Verify( comp );
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
            Assert.Equal( "1", result1 );
            Assert.Equal( "2", result2 );
        }
    }
}
