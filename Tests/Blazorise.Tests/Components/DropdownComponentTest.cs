using BasicTestApp.Client;
using Blazorise.Tests.Helpers;
using Bunit;
using Xunit;

namespace Blazorise.Tests.Components
{
    public class DropdownComponentTest : TestContext
    {
        public DropdownComponentTest()
        {
            BlazoriseConfig.AddBootstrapProviders( Services );
        }

        [Fact]
        public void CanShowAndHideDropdownComponent()
        {
            // setup
            var comp = RenderComponent<DropdownComponent>();
            var drpElement = comp.Find( "#dropdown" );
            var btnElement = comp.Find( "button" );
            var mnuElement = comp.Find( "#dropdown-menu" );

            // test
            btnElement.Click();

            // validate
            Assert.Contains( "show", drpElement.GetAttribute( "class" ) );
            Assert.Contains( "show", mnuElement.GetAttribute( "class" ) );

            // test
            btnElement.Click();

            // validate
            Assert.DoesNotContain( "show", drpElement.GetAttribute( "class" ) );
            Assert.DoesNotContain( "show", mnuElement.GetAttribute( "class" ) );
        }
    }
}
