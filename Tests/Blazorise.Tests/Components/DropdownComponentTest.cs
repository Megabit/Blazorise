using System;
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
            BlazoriseConfig.JSInterop.AddClosable( this.JSInterop );
            BlazoriseConfig.JSInterop.AddDropdown( this.JSInterop );
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
            this.JSInterop.VerifyInvoke( "initialize" );

            // test
            btnElement.Click();

            // validate
            this.JSInterop.VerifyInvoke( "destroy" );
        }
    }
}
