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
    public class ButtonOnlyComponentTest : TestContext
    {
        public ButtonOnlyComponentTest()
        {
            BlazoriseConfig.AddBootstrapProviders( Services );
        }

        [Fact]
        public Task RenderTest()
        {
            // test
            var comp = RenderComponent<ButtonOnlyComponent>();

            // validate
            return Verifier.Verify( comp );
        }
    }
}
